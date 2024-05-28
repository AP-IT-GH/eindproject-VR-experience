using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Collections;


public class capsuleAgent : Agent
{
    [Header("ML AGENT SETTINGS")]
    public float TooLateTimeSec = 30;

    [Header("OBJECTS")]
    public Transform Target;
    public Rigidbody rb;
    public BarrierSpawner Spawner;

    private Vector3 initPosition;
    private Quaternion initRotation;
    private void Start()
    {
        initPosition = gameObject.transform.localPosition;
        initRotation = gameObject.transform.localRotation;
    }
    
    private float speedMultiplier = 0.1f;
    private float rotationmultiplier;
    private float jumpForce = 10f;

    private Coroutine countdown;
    public override void OnEpisodeBegin() {

        //zet de agent op zijn plaats en collider aan.
        Spawner.SpawnRandomizedObjects();
        if (countdown != null)
            StopCoroutine(countdown);

        gameObject.GetComponent<Collider>().enabled = true;

        transform.localPosition = initPosition; 
        transform.localRotation = initRotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        speedMultiplier = 0.1f;
        rotationmultiplier = 2f;
        jumpForce = 10f;
        countdown = StartCoroutine(StartCountdown(TooLateTimeSec));
    }
    public override void CollectObservations(VectorSensor sensor) {
        sensor.AddObservation(this.transform.localPosition);//weet waar agent is
        sensor.AddObservation(Target.transform.localPosition);//weet waar target is
        // obstacles werken met rays
    }
    private void GiveAwardBasedOnDistance()
    {
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);
        float reward = ((distanceToTarget / 40) - 1) * -1;
        Debug.Log("Award given for distance: " + reward);
        AddReward(reward);
    }
    float currCountdownValue;
    public IEnumerator StartCountdown(float countdownValue = 50)
    {
        currCountdownValue = countdownValue;
        while (currCountdownValue > 0)
        {
            yield return new WaitForSeconds(1.0f);
            currCountdownValue--;
        }
        Debug.Log("Too late!");
        GiveAwardBasedOnDistance();
        EndEpisode();
    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //beweging
        Vector3 controlSignal = Vector3.zero;

        controlSignal.z = actionBuffers.ContinuousActions[1];
        transform.Translate(controlSignal * speedMultiplier);
        transform.Rotate(0.0f, rotationmultiplier * actionBuffers.ContinuousActions[0], 0.0f);
        float jumpAction = actionBuffers.ContinuousActions[2];
        //springen
        if (jumpAction > 0.5f && transform.position.y <= 0.5)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        //punten
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // target bereikt
        if (distanceToTarget < 1.42f)
        {
            SetReward(2.0f);
            EndEpisode();
            Debug.Log("We made it to the target!: Reward = 2");
        }
        if (transform.position.y < -5)
        {
            AddReward(-1f);
            GiveAwardBasedOnDistance();
            Debug.Log("Fell through a hole! Punishment: -1");
            EndEpisode();
        }
    }
    private bool startWebTouch = false;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "hole")
        {
            gameObject.GetComponent<Collider>().enabled = false;
            speedMultiplier = 0f;
            rotationmultiplier = 0f;
            jumpForce = -0.1f;
        }
    }
    private void OnTriggerEnter(Collider obstacle)
    {
        if (!startWebTouch)
        {
            AddReward(-0.5f);
            Debug.Log("Touched web! Punishment: -0.5");
        }
        if(obstacle.tag == "web")
        {
            startWebTouch = true;
            speedMultiplier = 0.01f;
        }
    }
    private void OnTriggerExit(Collider obstacle)
    {
        if (obstacle.tag == "web")
        {
            startWebTouch = false;
            speedMultiplier = 0.1f;
        }
    }
    //code zorgt dat de agents bewegingen getest kunnen worden.
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
        continuousActionsOut[2] = Input.GetKey(KeyCode.RightShift) ? 1f : 0f;
    }
}
