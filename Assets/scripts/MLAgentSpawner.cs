using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;

public class MLAgentSpawner : MonoBehaviour
{
    public GameObject MLAgent;
    public Transform SpawnBox;
    public GameObject Target;
    public BarrierSpawner Spawner;
    public GameObject GunKitPrefab;
    public GameObject StartEnvironment;

    [Header("How many to spawn initially.")]
    public int SpawnAmount;
    [Header("How many waves.")]
    public int Waves;
    [Header("The scale that the zombies grow by every wave.")]
    public float ScalePerWave;

    [Header("MENU'S")]
    public GameObject WinMenu;
    public GameObject StartMenu;
    public GameObject LoseMenu;

    private int currentWave;
    private bool paused = true;

    private GameObject gunKit;
    private GameObject gun;

    public void StartGame()
    {
        paused = false;

        StartMenu.SetActive(false);
        LoseMenu.SetActive(false);
        WinMenu.SetActive(false);

        gunKit = Instantiate(GunKitPrefab, StartEnvironment.transform);
        gun = gunKit.GetComponentInChildren<GunObject>().gameObject;
    }

    private void Update()
    {
        if (!paused)
            CheckIfZombieDied();
    }
    public void PlayerDied()
    {
        LoseMenu.SetActive(true);
        EndGame();
    }
    public void CheckIfZombieDied()
    {
        if (this.transform.childCount <= 0 && Waves > currentWave)
        {
            currentWave++;
            SpawnZombies();
        }
        else if (this.transform.childCount <= 0 && Waves <= currentWave)
        {
            WinMenu.SetActive(true);
            EndGame();
        }

    }
    private void EndGame()
    {
        paused = true;

        Destroy(gun);
        Destroy(gunKit);

        //Find any remaining ammo:
        Magazine[] magazines = FindObjectsOfType<Magazine>();
        foreach (Magazine item in magazines)
        {
            Destroy(item);
        }

        capsuleAgent[] zombies = gameObject.GetComponentsInChildren<capsuleAgent>();
        foreach (capsuleAgent item in zombies)
        {
            Destroy(item.gameObject);
        }
    }
    private void SpawnZombies()
    {

        Spawner.SpawnRandomizedObjects();

        int amountOfZombies = Convert.ToInt32(SpawnAmount + (currentWave * ScalePerWave));
        for (int i = 0; i < amountOfZombies; i++)
        {
            //Random position!
            GameObject newObject = Instantiate(MLAgent, this.transform);
            newObject.transform.position = RandomPointInBox(SpawnBox.transform.position, newObject.transform.position, SpawnBox.GetComponent<MeshRenderer>().bounds.size);
            
            capsuleAgent capsuleAgent = newObject.GetComponent<capsuleAgent>();
            capsuleAgent.Target = Target;
            //capsuleAgent.AgentGameSpawner = this;

        }
    }

    private static Vector3 RandomPointInBox(Vector3 center, Vector3 objectBasisPos, Vector3 size)
    {
        return center + new Vector3(
           (UnityEngine.Random.value - 0.5f) * size.x,
           -size.y + 0.5f + objectBasisPos.y,
           (UnityEngine.Random.value - 0.5f) * size.z
        ); ;
    }
}
