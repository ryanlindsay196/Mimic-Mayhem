using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnPoint : MonoBehaviour {
    ItemSpawnZone itemSpawnZone;//set automatically to whatever spawn zone this object resides in
    bool itemSpawned;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Eatable[] items = FindObjectsOfType<Eatable>();
        itemSpawned = false;
        for(int i = 0; i < items.Length - 1; i++)
        {
            if ((items[i].transform.position - transform.position).magnitude <= .04f && items[i].enabled)
            {
                ItemSpawned = true;
            }
        }
	}

    public bool ItemSpawned
    {
        get { return itemSpawned; }
        set { itemSpawned = value; }
    }

    public void SpawnItem(GameObject itemToSpawn)
    {
        if (!itemSpawned)
        {
            Instantiate(itemToSpawn, transform.position, transform.rotation);
            itemSpawned = true;
            Debug.Log("Spawned: " + itemToSpawn.name);
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("Spawn point entered spawn zone");
    //    if(collision.gameObject.GetComponent<ItemSpawnZone>() != null)
    //    {
    //        if(itemSpawnZone == null)
    //        {
    //            itemSpawnZone = collision.gameObject.GetComponent<ItemSpawnZone>();
    //            itemSpawnZone.AddSpawnPoint(GetComponent<ItemSpawnPoint>());
    //        }

    //    }
    //}
}
