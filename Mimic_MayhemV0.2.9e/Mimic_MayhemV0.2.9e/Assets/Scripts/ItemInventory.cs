using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInventory : MonoBehaviour {

	// Use this for initialization

    public List<ItemBase> ItemList;


	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddItem(ItemBase item)
    {
        ItemList.Add(item);
    }
}
