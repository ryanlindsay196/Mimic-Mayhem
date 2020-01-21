using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour {

    Pickupable pickupable;

    // Use this for initialization
	void Start () {
        pickupable = GetComponent<Pickupable>(); 
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.P)){
            //Destroy;
        }
	}
}
