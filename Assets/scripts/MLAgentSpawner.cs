using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MLAgentSpawner : MonoBehaviour
{
    public GameObject MLAgent;
    public Transform SpawnBox;
    public GameObject Target;
    public BarrierSpawner Spawner;
    public GameObject startScreen;

    [Header("How many to spawn initially.")]
    public int SpawnAmount;
    [Header("How many waves.")]
    public int Waves;
    [Header("The scale that the zombies grow by every wave.")]
    public float ScalePerWave;

    [Header("PLAYER WIN.")]
    public GameObject GunKit;
    public GameObject Gun;
    public Canvas UICanvas;
    public Text StatusText;

    private int currentWave;
    private bool done = false;
    private bool gameStarted = false;

    public void StartGame()
    {
        Debug.Log("StartGame method called."); 
        gameStarted = true; 
        startScreen.SetActive(false);
        SpawnZombies();
    }

    private void Update()
    {
        if (!done && gameStarted)
            CheckIfZombieDied();
    }
    public void PlayerDied()
    {
        Debug.Log("You lose!");
        EndGame();
    }
    //public void CheckIfZombieDied()
    //{
    //    if (gameStarted)
    //    {
    //        currentWave++;
    //        SpawnZombies();
    //    } else if (this.transform.childCount <= 0 && Waves <= currentWave)
    //    {
    //        Debug.Log("You win!");
    //        //Update text to YOU WIN!
    //        EndGame();
    //    }
    //}

    public void CheckIfZombieDied()
    {
        if (this.transform.childCount <= 0 && Waves > currentWave)
        {
            currentWave++;
            SpawnZombies();
        }
        else if (this.transform.childCount <= 0 && Waves <= currentWave)
        {
            done = true;
            Debug.Log("You win!");
            //Update text to YOU WIN!
            EndGame();
        }

    }
    private void EndGame()
    {
        done = true;

        Destroy(Gun);
        Destroy(GunKit);
        //Find any remaining ammo:
        Magazine[] magazines = FindObjectsOfType<Magazine>();
        foreach (Magazine item in magazines)
        {
            Destroy(item);
        }
        //Show win or lose UI.

        Destroy(this.gameObject);
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
            capsuleAgent.AgentGameSpawner = this;

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
