using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : MonoBehaviour {

	// Use this for initialization
    public string ItemName = "NAMELESS_ITEM";
    public GameObject PickupItemPrefab;
    public Sprite ItemHUDSprite = null;



	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public ItemBase Clone()
    {
        return (ItemBase)MemberwiseClone();
    }
}
