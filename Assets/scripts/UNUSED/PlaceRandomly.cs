using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlaceRandomly : MonoBehaviour
{
    public List<GameObject> gameObjects;

    public GameObject spawnBox;

    public int AmountToSpawn;
    private int spawnIndex;
    private int spawnAmount;

    public void SpawnRandomly()
    {
        if (spawnAmount <= AmountToSpawn)
        {
            int randindex = Random.Range(0, gameObjects.Count);

            //Generate random position (within box)

            GameObject randomPrefab = gameObjects[randindex];

            GameObject newObject = Instantiate(randomPrefab, RandomPointInBox(
                spawnBox.transform.position,
                randomPrefab.transform.position,
                spawnBox.GetComponent<MeshRenderer>().bounds.size),
                randomPrefab.transform.rotation
            );

            newObject.GetComponent<OverlapScript>().placeRandomly = this;
            newObject.GetComponent<OverlapScript>().spawnIndex = spawnIndex;

            spawnIndex++;
            spawnAmount++;
        }
    }

    public void SpawnFailed()
    {
        spawnAmount--;
        SpawnRandomly();
    }

    private static Vector3 RandomPointInBox(Vector3 center, Vector3 objectBasisPos, Vector3 size)
    {
        return center + new Vector3(
           (Random.value - 0.5f) * size.x,
           -size.y + 0.5f + objectBasisPos.y,
           (Random.value - 0.5f) * size.z
        ); ;
    }



    private void Start()
    {
        for (int i = 0; i < AmountToSpawn; i++)
        {
            SpawnRandomly();
        }   
    }
}
