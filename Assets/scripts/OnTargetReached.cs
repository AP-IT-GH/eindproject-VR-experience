using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTargetReached : MonoBehaviour
{
    public float threshold = 0.02f;
    public Transform target;
    public SimpleShoot gun;
    private bool wasReached = false;

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, target.position);

        if(distance < threshold && !wasReached)
        {
            gun.Slide();

            wasReached = true;
        }
        else if(distance >= threshold)
        {
            wasReached = false;
        }
    }
}
