using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;


public class capsuleAgent : Agent
{
    public Transform Target;

    private float speedMultiplier = 0.1f;
    private float rotationmultiplier = 1f;
    private float jumpForce = 10f;

    private bool middebereikt = false;
    private bool bijnaDaar = false;
    public Rigidbody rb;
    public override void OnEpisodeBegin() {

        //zet de agent op zijn plaats en collider aan.
        gameObject.GetComponent<Collider>().enabled = true;
        this.transform.localPosition = new Vector3(16f, 0.8f, 0);//start plaats =(23f, 0.8f, tussen -7 en 9)
        this.transform.localRotation = Quaternion.identity;// zet op standaart locatie
        this.transform.Rotate(0.0f, -90f, 0.0f);//draai -90 graden 
        speedMultiplier = 0.1f;
        rotationmultiplier = 1f;
        jumpForce = 10f;

}
    public override void CollectObservations(VectorSensor sensor) {
        sensor.AddObservation(this.transform.localPosition);//weet waar agent is
        sensor.AddObservation(Target.transform.localPosition);//weet waar target is
        // obstacles werken met rays
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
            SetReward(1.0f);
            EndEpisode();
        }
      
        // in de juiste richting aan het gaan
        if (transform.position.x <= 11.5 && middebereikt == false)
        {
            SetReward(0.3f);
            middebereikt = true;
        }
        if (transform.position.x <= 6 && bijnaDaar == false)
        {
            SetReward(0.5f);
            bijnaDaar = true;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
    if (collision.gameObject.tag == "hole")
        {
            gameObject.GetComponent<Collider>().enabled = false;
            speedMultiplier = 0f;
            rotationmultiplier = 0f;
            jumpForce = -0.1f;
            SetReward(-0.5f);
            EndEpisode();
            Debug.Log(speedMultiplier);
        }
    }
    private void OnTriggerEnter(Collider obstacle)
    {
        if(obstacle.tag == "web")
        {
            speedMultiplier = 0.01f;
            Debug.Log(speedMultiplier);
        }
    }
    private void OnTriggerExit(Collider obstacle)
    {
        if (obstacle.tag == "web")
        {
            speedMultiplier = 0.1f;
            Debug.Log(speedMultiplier);
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
