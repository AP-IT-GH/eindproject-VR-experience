using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlapScript : MonoBehaviour
{
    public PlaceRandomly placeRandomly;

    public int spawnIndex = 0;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.collider.gameObject.layer);
        if (collision.collider.gameObject.layer == 8 && collision.collider.gameObject.GetComponent<OverlapScript>().spawnIndex < this.spawnIndex)
        {
            //placeRandomly.SpawnFailed();
            Destroy(gameObject);
        }
    }
}
