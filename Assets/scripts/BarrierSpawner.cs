using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierSpawner : MonoBehaviour
{
    // Array om de prefabs op te slaan die gespawned kunnen worden
    public GameObject[] prefabs;

    void Start()
    {
        // Kies een random child van dit GameObject
        int childIndex = Random.Range(0, transform.childCount);
        Transform spawnLocation = transform.GetChild(childIndex);

        // Kies een random prefab uit de array
        int prefabIndex = Random.Range(0, prefabs.Length);
        GameObject selectedPrefab = prefabs[prefabIndex];

        // Spawn de geselecteerde prefab op de locatie van het gekozen child
        Instantiate(selectedPrefab, new Vector3(spawnLocation.position.x, 0.02f, spawnLocation.position.z), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
