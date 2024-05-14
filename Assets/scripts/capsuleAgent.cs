using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;


public class capsuleAgent : Agent
{
    public Transform Target;
    public override void OnEpisodeBegin() {
        
      //zet de agent op zijn plaats.

            this.transform.localPosition = new Vector3(-16f, 0.8f, 9); this.transform.localRotation = Quaternion.identity;
        
    }
    public override void CollectObservations(VectorSensor sensor) {
        sensor.AddObservation(this.transform.localPosition);//weet waar agent is
        sensor.AddObservation(Target.transform.localPosition);//weet waar target is
        // obstacles werken met rays
    }
    public float speedMultiplier = 0.1f;
    public float rotationmultiplier = 1f;
    public float jumpForce = 0.1f;
    public Rigidbody rb;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //beweging
        Vector3 controlSignal = Vector3.zero;

        controlSignal.z = actionBuffers.ContinuousActions[1];
        transform.Translate(controlSignal * speedMultiplier);
        transform.Rotate(0.0f, rotationmultiplier * actionBuffers.ContinuousActions[0], 0.0f);
        float jumpAction = actionBuffers.ContinuousActions[2];
        //springen
        if (jumpAction > 0.5f && transform.position.y <= 2)
        {
            

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            
        }
        //punten
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // target bereikt
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            EndEpisode();
        }


    }

    //code zorgt dat de agents bewegingen getest kunnen worden.
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
        continuousActionsOut[2] = Input.GetKey(KeyCode.RightShift) ? 1f : 0f; ;
    }
}
