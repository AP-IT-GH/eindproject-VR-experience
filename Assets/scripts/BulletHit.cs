using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHit : MonoBehaviour
{
    public SimpleShoot gun;
    private void OnCollisionEnter(Collision collision)
    {
        gun.Hit(collision);

        Destroy(gameObject);
    }
}
