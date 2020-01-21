using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour {

    MimicMain[] playersObjects;
    public bool spawnPointIsAccessible;//if true, then the player can spawn here

	// Use this for initialization
	void Start () {
        playersObjects = FindObjectsOfType<MimicMain>();
	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < playersObjects.Length; i++)
        {
            if (Vector3.Distance(playersObjects[i].transform.position, transform.position) < 3)
            {
                spawnPointIsAccessible = false;
            }
            else
                spawnPointIsAccessible = true;
        }
	}
}
