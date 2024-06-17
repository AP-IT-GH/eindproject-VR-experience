﻿using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

[AddComponentMenu("Nokobot/Modern Guns/Simple Shoot")]
public class SimpleShoot : MonoBehaviour
{
    [Header("Prefab Refrences")]
    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public GameObject muzzleFlashPrefab;

    [Header("Location Refrences")]
    [SerializeField] private Animator gunAnimator;
    [SerializeField] private Transform barrelLocation;
    [SerializeField] private Transform casingExitLocation;

    [Header("Settings")]
    [Tooltip("Specify time to destory the casing object")] [SerializeField] private float destroyTimer = 2f;
    [Tooltip("Bullet Speed")] [SerializeField] private float shotPower = 500f;
    [Tooltip("Casing Ejection Speed")] [SerializeField] private float ejectPower = 150f;

    public AudioSource source;
    public AudioClip fireSound;
    public AudioClip reload;
    public AudioClip noAmmo;

    public GameObject AmmoSpawnLocation;
    public GameObject AmmoPrefab;

    public Magazine magazine;
    public XRBaseInteractor socketInteractor;
    private bool bulletInGun = false;

    public Vector3 MagazineInGunColliderPosition;
    public Vector3 MagazineInGunColliderScale;

    private Vector3 magazineOriginalCenter;
    private Vector3 magazineOriginalScale;

    public void AddMagazine(SelectEnterEventArgs interactable)
    {
        GameObject interactableObject = interactable.interactableObject.transform.gameObject;
        BoxCollider collider = interactableObject.GetComponentInChildren<BoxCollider>();

        magazineOriginalCenter = collider.center;
        magazineOriginalScale = collider.size;

        collider.center = MagazineInGunColliderPosition;
        collider.size = MagazineInGunColliderScale;

        magazine = interactableObject.GetComponent<Magazine>();

        source.PlayOneShot(reload);
    }
    public void RemoveMagazine(SelectExitEventArgs interactable)
    {
        GameObject interactableObject = interactable.interactableObject.transform.gameObject;
        BoxCollider collider = interactableObject.GetComponentInChildren<BoxCollider>();

        collider.center = magazineOriginalCenter;
        collider.size = magazineOriginalScale;

        magazine = null;
        source.PlayOneShot(reload);
    }
    public void Slide()
    {
        bulletInGun = true;
        magazine.numberOfBullet--;
        source.PlayOneShot(reload);
    }
    void Start()
    {
        socketInteractor.selectEntered.AddListener(AddMagazine);
        socketInteractor.selectExited.AddListener(RemoveMagazine);

        if (barrelLocation == null)
            barrelLocation = transform;

        if (gunAnimator == null)
            gunAnimator = GetComponentInChildren<Animator>();
    }

    public void PullTheTrigger()
    {
        if (magazine && magazine.numberOfBullet >= 0 && bulletInGun)
        {
            gunAnimator.SetTrigger("Fire");
        }
        else
        {
            if (bulletInGun)
                Instantiate(AmmoPrefab, AmmoSpawnLocation.transform);

            bulletInGun = false;
            source.PlayOneShot(noAmmo);
        }
    }

    //This function creates the bullet behavior
    public bool shooting = false;
    void Shoot()
    {
        magazine.numberOfBullet--;

        source.PlayOneShot(fireSound);

        if (muzzleFlashPrefab)
        {
            //Create the muzzle flash
            GameObject tempFlash;
            tempFlash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);

            //Destroy the muzzle flash effect
            Destroy(tempFlash, destroyTimer);
        }

        //cancels if there's no bullet prefeb
        if (!bulletPrefab)
        { return; }

        // Create a bullet and add force on it in direction of the barrel
        shooting = true;

        GameObject bullet = Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(barrelLocation.forward * shotPower);
        bullet.AddComponent<BulletHit>().gun = this;
    }

    public void Hit(Collision hit)
    {
        if (hit.gameObject.CompareTag("Zombie"))
        {
            Animator zombieAnimator = hit.gameObject.GetComponent<Animator>();
            capsuleAgent agent = hit.gameObject.GetComponent<capsuleAgent>();

            if (zombieAnimator != null)
            {
                Destroy(agent);
                zombieAnimator.SetBool("IsHit", true);
                StartCoroutine(DestroyZombieAfterAnimation(hit.gameObject, zombieAnimator));
            }
        }

    }

    private IEnumerator DestroyZombieAfterAnimation(GameObject zombie, Animator zombieAnimator)
    {
        
        yield return new WaitForSeconds(3f);

        Destroy(zombie);
    }

    //This function creates a casing at the ejection slot
    void CasingRelease()
    {
        //Cancels function if ejection slot hasn't been set or there's no casing
        if (!casingExitLocation || !casingPrefab)
        { return; }

        //Create the casing
        GameObject tempCasing;
        tempCasing = Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation) as GameObject;
        //Add force on casing to push it out
        tempCasing.GetComponent<Rigidbody>().AddExplosionForce(Random.Range(ejectPower * 0.7f, ejectPower), (casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f), 1f);
        //Add torque to make casing spin in random direction
        tempCasing.GetComponent<Rigidbody>().AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(100f, 1000f)), ForceMode.Impulse);

        //Destroy casing after X seconds
        Destroy(tempCasing, destroyTimer);
    }


}
