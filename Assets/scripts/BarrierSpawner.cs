using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierSpawner : MonoBehaviour
{
    // Array om de prefabs op te slaan die gespawned kunnen worden
    public GameObject[] prefabs;
    public void SpawnRandomizedObjects()
    {
        //Loop through each child.
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            Transform item = gameObject.transform.GetChild(i);
            Transform spawnedObjectsFolder = item.Find("SpawnedObjects");
            //Clear up any existing objects
            for (int j = 0; j < spawnedObjectsFolder.childCount; j++)
            {
                Destroy(spawnedObjectsFolder.GetChild(j).gameObject);
            }

            // Kies een random child van dit GameObject
            int childIndex;
            Transform spawnLocation;
            do
            {
                childIndex = Random.Range(0, item.childCount);
                spawnLocation = item.GetChild(childIndex);
            } while (spawnLocation.name == "SpawnedObjects");

            // Kies een random prefab uit de array
            int prefabIndex = Random.Range(0, prefabs.Length);
            GameObject selectedPrefab = prefabs[prefabIndex];

            // Spawn de geselecteerde prefab op de locatie van het gekozen child
            GameObject prefab = Instantiate(selectedPrefab, new Vector3(spawnLocation.position.x, 0.02f, spawnLocation.position.z), Quaternion.identity);
            prefab.transform.parent = spawnedObjectsFolder;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
