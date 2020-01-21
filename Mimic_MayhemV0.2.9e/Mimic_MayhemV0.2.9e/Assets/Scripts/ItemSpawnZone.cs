using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnZone : MonoBehaviour {
    [SerializeField]
    List<GameObject> itemSpawnTable;
    [SerializeField]
    float spawnTimer;
    [SerializeField]
    int maxSpawnedItems;

    [SerializeField]
    float maxSpawnTimerResetValue;//when spawnTimer reaches 0, randomly select a time between maxSpawnTimerResetValue and minSpawnTimerResetValue
    [SerializeField]
    float minSpawnTimerResetValue;//when spawnTimer reaches 0, randomly select a time between maxSpawnTimerResetValue and minSpawnTimerResetValue

    [SerializeField]
    List<ItemSpawnPoint> spawnPoints;//Fill this in the editor
	// Use this for initialization
	void Start () {
        //spawnPoints = FindObjectsOfType<>
        ResetTimer();
	}

    void ResetTimer()
    {
        spawnTimer = Random.Range(minSpawnTimerResetValue, maxSpawnTimerResetValue);
        SpawnItem();
    }

    //public void AddSpawnPoint(ItemSpawnPoint spawnPointToAdd)
    //{
    //    spawnPoints.Add(spawnPointToAdd);
    //    //spawn points are never added to the list
    //}

    void SpawnItem()
    {
        int spawnedItemsCount = 0;
        for (int i = 0; i < maxSpawnedItems; i++)
        {//count the spawned items
            if (spawnPoints[i].ItemSpawned)
                spawnedItemsCount++;
        }
        if(spawnedItemsCount < maxSpawnedItems)
        {//if under item limit for area
            int spawnIndex = Random.Range(0, maxSpawnedItems - spawnedItemsCount - 1) + 1;
            int emptySpawnPoints = 0;
            for (int i = 0; i < spawnPoints.Count; i++)
            {//spawn at a random empty spawn point
                if (!spawnPoints[i].ItemSpawned)
                    emptySpawnPoints++;
                if(emptySpawnPoints == spawnIndex)
                {
                    Debug.Log("Spawning item...");
                    spawnPoints[i].SpawnItem(itemSpawnTable[(int)Random.Range(0, itemSpawnTable.Count)]);
                    break;
                }
            }
        }
            //Select spawn empty point, then spawn item
    }
	
	// Update is called once per frame
	void Update () {
        if (spawnTimer <= 0)
            ResetTimer();
        spawnTimer -= Time.deltaTime;
	}
}
