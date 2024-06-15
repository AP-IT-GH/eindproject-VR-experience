using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierSpawner : MonoBehaviour
{
    // Array om de prefabs op te slaan die gespawned kunnen worden
    public GameObject[] prefabs;

    public void SpawnRandomizedObjects()
    {
        // Loop through each child (Barrier1, Barrier2, etc.).
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            Transform barrier = gameObject.transform.GetChild(i);
            Transform spawnedObjectsFolder = barrier.Find("SpawnedObjects");
            // Clear up any existing objects
            for (int j = 0; j < spawnedObjectsFolder.childCount; j++)
            {
                Destroy(spawnedObjectsFolder.GetChild(j).gameObject);
            }

            // Kies een random child van dit GameObject (Spawn1, Spawn2).
            int childIndex;
            Transform spawnLocation;
            do
            {
                childIndex = Random.Range(0, barrier.childCount);
                spawnLocation = barrier.GetChild(childIndex);
            } while (spawnLocation.name == "SpawnedObjects");

            // Spawn een random prefab op de eerste locatie
            int prefabIndex = Random.Range(0, prefabs.Length - 1);
            GameObject selectedPrefab = prefabs[prefabIndex];
            GameObject prefab = Instantiate(selectedPrefab, new Vector3(spawnLocation.position.x, 0.02f, spawnLocation.position.z), Quaternion.identity);
            prefab.transform.parent = spawnedObjectsFolder;
            prefab.name = barrier.name[7] + selectedPrefab.name;  // Set the name (Barrier1 -> 1PrefabName)

            // Bepaal de andere spawnlocatie
            Transform otherSpawnLocation = null;
            foreach (Transform child in barrier)
            {
                if (child != spawnLocation && child.name != "SpawnedObjects")
                {
                    otherSpawnLocation = child;
                    break;
                }
            }

            // Spawn de laatste prefab op de andere locatie
            GameObject lastPrefab = prefabs[prefabs.Length - 1];
            GameObject lastPrefabInstance = Instantiate(lastPrefab, new Vector3(otherSpawnLocation.position.x, 0.02f, otherSpawnLocation.position.z), Quaternion.identity);
            lastPrefabInstance.transform.parent = spawnedObjectsFolder;
            lastPrefabInstance.name = barrier.name[7] + lastPrefab.name;  // Set the name (Barrier1 -> 1PrefabName2) 
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
