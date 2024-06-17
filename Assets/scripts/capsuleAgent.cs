using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Collections;
using System;
using static UnityEngine.GraphicsBuffer;
using Unity.Mathematics;


public class capsuleAgent : Agent
{
    // Keeps the current checkpoint number, at the end of the training we'll calculate the award based on this.
    // (This prevents triggers causing multiple awards to be given)
    private int checkpoint = 0;

    [Tooltip("To enable or disable logging")]
    public bool Verbose = true;

    [Header("ML AGENT SETTINGS")]

    [Tooltip("Destroys the agent when the episode ends.")]
    public bool DestroyOnEnd = false;

    [Tooltip("The checkpoint the ML agent starts at.")]
    public int StartCheckpoint;

    [Tooltip("Ends the episode when the ML agent went back a checkpoint.")]
    public bool EndWhenGoingBack = true;

    [Tooltip("Ends the episode when the ML agent touches a wall.")]
    public bool EndWhenTouchingWall = true;

    [Header("AWARDS")]
    
    [Tooltip("The award given for passing a singular checkpoint. (stacks)")]
    public float CheckpointAward = 0.25F;

    [Tooltip("The punishment given for falling into a hole.")]
    public float TargetAward = 1F;

    [Header("PUNISHMENTS")]

    [Tooltip("The punishment for touching a wall.")]
    public float PunishmentWallTouch = 2;

    [Tooltip("The punishment for going back (when enabled).")]
    public float PunishmentGoingBack = 2;

    [Tooltip("The punishment given for touching a web.")]
    public float ObstacleTouchedPunishmentWeb = 0.5F;

    [Tooltip("The punishment given for falling into a hole.")]
    public float ObstacleTouchedPunishmentHole = 1F;

    [Tooltip("The jumping punishment.")]
    public float JumpingPunishment = 0.05F;

    [Tooltip("The punishment per step taken.")]
    public float StepPunishment = 0.00005F;

    [Header("OBJECTS")]
    public Transform TargetStart;
    public Transform TargetEnd;
    public MLAgentSpawner AgentGameSpawner;

    public Transform Target;
    public Rigidbody rb;
    public BarrierSpawner Spawner;

    private Vector3 initPosition;
    private Quaternion initRotation;
    private Vector3 targetInitPosition;
    private void Start()
    {
        targetInitPosition = Target.localPosition;
        initPosition = gameObject.transform.localPosition;
        initRotation = gameObject.transform.localRotation;
    }
    
    private float speedMultiplier = 0.1f;
    private float rotationmultiplier;
    private float jumpForce = 5f;

    private TouchObjectType currentlyTouching = TouchObjectType.NOTHING;
    public override void OnEpisodeBegin() {
        currentlyTouching = TouchObjectType.NOTHING;

        if (TargetEnd != null && TargetStart != null)
            Target.localPosition = new Vector3(
                targetInitPosition.x, 
                targetInitPosition.y, 
                UnityEngine.Random.Range(
                    TargetStart.localPosition.z,
                    TargetEnd.localPosition.z
                    )
                );

        checkpoint = StartCheckpoint;

        //zet de agent op zijn plaats en collider aan.
        if (Spawner != null)
            Spawner.SpawnRandomizedObjects();

        gameObject.GetComponent<Collider>().enabled = true;

        transform.localPosition = initPosition; 
        transform.localRotation = initRotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        //Annoying... it wont SET!!!
        webTouching = false;
        speedMultiplier = 0.1f;

        rotationmultiplier = 2f;
        jumpForce = 5f;
    }
    public override void CollectObservations(VectorSensor sensor) {
        //Distance between target and agent.
        sensor.AddObservation(Vector3.Distance(Target.transform.localPosition, this.transform.localPosition));

        //Position of target and itself
        sensor.AddObservation(Target.transform.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        //The current checkpoint we are at
        sensor.AddObservation(checkpoint);

        //What we are touching currently
        sensor.AddObservation((int) currentlyTouching);
        // obstacles werken met rays
    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //checkRayCast();

        transform.Translate(discreteActionsToMovementVector(actionBuffers.DiscreteActions) * speedMultiplier);
        transform.Rotate(discreteActionsToRotationVector(actionBuffers.DiscreteActions) * rotationmultiplier);

        int jumpAction = actionBuffers.DiscreteActions[(int)discreteType.JUMP];

        if (jumpAction == 1 && transform.position.y <= 0.5)
        {
            AddReward(-JumpingPunishment);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // Reached target.
        if (distanceToTarget < 1.42f)
        {
            AddReward(TargetAward);

            if (Verbose)
                Debug.Log("We made it to the target!: " + TargetAward);

            if (AgentGameSpawner != null)
                AgentGameSpawner.PlayerDied();

            EpisodeEnd();
        }

        // Fell through hole.
        if (transform.position.y < -5)
        {
            EpisodeEnd();
        }

        if (this.MaxStep <= this.StepCount)
        {
            if (Verbose)
                Debug.Log("Max steps reached!");

            EpisodeEnd();
        }

        //This bug is so annoying...
        if (!webTouching)
        {
            speedMultiplier = 0.1f;
        }

        CalculateRewardsAndPunishmentsPerStep();
    }
    private void checkRayCast()
    {
        RayPerceptionSensorComponent3D m_rayPerceptionSensorComponent3D = GetComponent<RayPerceptionSensorComponent3D>();

        var rayOutputs = RayPerceptionSensor.Perceive(m_rayPerceptionSensorComponent3D.GetRayPerceptionInput()).RayOutputs;
        int lengthOfRayOutputs = rayOutputs.Length;

        // Alternating Ray Order: it gives an order of
        // (0, -delta, delta, -2delta, 2delta, ..., -ndelta, ndelta)
        // index 0 indicates the center of raycasts
        for (int i = 0; i < lengthOfRayOutputs; i++)
        {
            GameObject goHit = rayOutputs[i].HitGameObject;
            if (goHit != null)
            {
                var rayDirection = rayOutputs[i].EndPositionWorld - rayOutputs[i].StartPositionWorld;
                var scaledRayLength = rayDirection.magnitude;
                float rayHitDistance = rayOutputs[i].HitFraction * scaledRayLength;

                // Print info:
                string dispStr = "";
                dispStr = dispStr + "__RayPerceptionSensor - HitInfo__:\r\n";
                dispStr = dispStr + "GameObject name: " + goHit.name + "\r\n";
                dispStr = dispStr + "Hit distance of Ray: " + rayHitDistance + "\r\n";
                dispStr = dispStr + "GameObject tag: " + goHit.tag + "\r\n";
                Debug.Log(dispStr);
            }
        }
    }
    /*
     (Values start from 0)

     MOVEMENT (2 STATES): FORWARDS, NO ACTION
     ROTATION (3 STATES): LEFT, NO ACTION, RIGHT
     JUMP (2 STATES): JUMP, NO ACTION
     */
    private enum discreteType
    {
        MOVEMENT,
        ROTATION,
        JUMP
    }
    private Vector3 discreteActionsToMovementVector(ActionSegment<int> discreteActions)
    {
        Vector3 movement = new Vector3(
            0, 
            0,
            discreteActions[(int)discreteType.MOVEMENT] * 1
         );
        return movement;
    }
    private Vector3 discreteActionsToRotationVector(ActionSegment<int> discreteActions)
    {
        Vector3 rotation = new Vector3(
            0,
            discreteActions[(int)discreteType.ROTATION]-1,
            0
        );
        return rotation;
    }
    //Calculates the rewards PER STEP.
    private void CalculateRewardsAndPunishmentsPerStep()
    {
        //Punishment per step
        AddReward(-(StepPunishment));
    }

    private void EpisodeEnd()
    {
        if (DestroyOnEnd)
            Destroy(gameObject);
        EndEpisode();
    }

    private bool isTouching = false;
    private bool webTouching = false;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "hole")
        {
            if (!isTouching)
            {
                isTouching = true;

                AddReward(-ObstacleTouchedPunishmentHole);

                if (Verbose)
                    Debug.Log("Fell through a hole! Punishment: " + (-ObstacleTouchedPunishmentHole));
            }

            currentlyTouching = TouchObjectType.HOLE;

            gameObject.GetComponent<Collider>().enabled = false;
            speedMultiplier = 0f;
            rotationmultiplier = 0f;
            jumpForce = -0.1f;
        } else if (collision.gameObject.tag == "wall")
        {
            currentlyTouching = TouchObjectType.WALL;

            if (!isTouching)
            {
                isTouching = true;

                if (Verbose)
                    Debug.Log("Touched a wall!");

                AddReward(-PunishmentWallTouch);

                if (EndWhenTouchingWall)
                {
                    EpisodeEnd();
                }
            }
        }
    }
    private void OnTriggerEnter(Collider obstacle)
    {
        if (obstacle.tag == "goodrewardbox" || obstacle.tag == "web")
        {
            switch (obstacle.tag)
            {
                case "goodrewardbox":
                    currentlyTouching = TouchObjectType.CHECKPOINT;
                    break;
                case "web":
                    currentlyTouching = TouchObjectType.WEB;
                    break;
            }

            Checkpoint checkpointObstacle = obstacle.gameObject.GetComponent<Checkpoint>();

            if (checkpoint > checkpointObstacle.CheckpointNumber)
            {
                webTouching = false;

                if (Verbose)
                    Debug.Log("We went back! Punishment: " + (-PunishmentGoingBack));

                AddReward(-PunishmentGoingBack);

                if (EndWhenGoingBack)
                {
                    EndEpisode();
                }
            } else if (checkpoint != checkpointObstacle.CheckpointNumber) {

                webTouching = false;

                if (Verbose)
                    Debug.Log("New checkpoint " + checkpoint + " reached! Reward: " + CheckpointAward);

                AddReward(CheckpointAward);
            }

            checkpoint = checkpointObstacle.CheckpointNumber;
        }
        
        if (obstacle.tag == "web" && !webTouching)
        {
            webTouching = true;

            if (!isTouching)
            {
                isTouching = true;

                if (Verbose)
                    Debug.Log("We touched a web! " + (-ObstacleTouchedPunishmentWeb));

                AddReward(-ObstacleTouchedPunishmentWeb);
            }
            speedMultiplier = 0.01f;
        }

        if (obstacle.tag == "floor")
        {
            webTouching = false;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        isTouching = false;
        currentlyTouching = TouchObjectType.NOTHING;
    }
    private void OnTriggerExit(Collider obstacle)
    {
        isTouching = false;

        currentlyTouching = TouchObjectType.NOTHING;

        if (obstacle.tag == "web" && webTouching)
        {
            webTouching = false;
            speedMultiplier = 0.1f;
        }
    }
    //code zorgt dat de agents bewegingen getest kunnen worden.
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;

        discreteActions[(int)discreteType.JUMP] = Convert.ToInt32(Input.GetKey(KeyCode.RightShift));

        discreteActions[ (int)discreteType.ROTATION ] = Convert.ToInt32(Input.GetAxis("Horizontal")) + 1;
        discreteActions[ (int)discreteType.MOVEMENT ] = Convert.ToInt32(Input.GetAxis("Vertical"));
    }
}

public enum TouchObjectType
{
    NOTHING,
    WALL,
    CHECKPOINT,
    WEB,
    HOLE,
}