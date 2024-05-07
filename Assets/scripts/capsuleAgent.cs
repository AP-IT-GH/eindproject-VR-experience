using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;


public class capsuleAgent : Agent
{
    public Transform Target;
    public override void OnEpisodeBegin() {
        if (this.transform.localPosition.y < 0)
        {

            this.transform.localPosition = new Vector3(0, 0.8f, 0); this.transform.localRotation = Quaternion.identity;
        }
    }
    public override void CollectObservations(VectorSensor sensor) {
        sensor.AddObservation(this.transform.localPosition);

    }
    public float speedMultiplier = 0.1f;
    public float rotationmultiplier = 3f;
    public float jumpForce = 5f;
    public Rigidbody rb;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {

        Vector3 controlSignal = Vector3.zero;

        controlSignal.z = actionBuffers.ContinuousActions[1];
        transform.Translate(controlSignal * speedMultiplier);
        transform.Rotate(0.0f, rotationmultiplier * actionBuffers.ContinuousActions[0], 0.0f);
        float jumpAction = actionBuffers.ContinuousActions[2];

        if (jumpAction > 0.5f && transform.position.y <= 2)
        {
            

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
        continuousActionsOut[2] = Input.GetKey(KeyCode.RightShift) ? 1f : 0f; ;
    }
}
