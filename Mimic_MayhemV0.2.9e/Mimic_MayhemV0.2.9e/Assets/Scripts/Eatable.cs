using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eatable : MonoBehaviour {

    public bool IsEaten;

    [SerializeField]
    public enum ItemType { food, weapon, treasure, household, none }
    [SerializeField]
    int pointValue;
    public ItemType itemType;
    [SerializeField]
    int hoardID;
    [SerializeField]
    string itemName;

    public int GetHoardID
    {
        get { return hoardID; }
    }

    public int GetPointValue
    {
        get { return pointValue; }
    }

    public int SetPointValue
    {
        set { pointValue = value; }
    }

    public string GetItemName
    {
        get { return itemName; }
    }

	// Use this for initialization
	void Start () {
        IsEaten = false;

        if(itemType == ItemType.treasure)
        {
            MimicSense mimicSense = FindObjectOfType<MimicSense>();
            mimicSense.SetText(itemName + " Spawned!");
            Debug.Log(itemName + " Spawned!");
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GetEaten()
    {
        //Debug.Log("Eaten;");
        IsEaten = true;
        if(GetComponent<MeshRenderer>() != null)
            GetComponent<MeshRenderer>().gameObject.SetActive(false);
        FindObjectOfType<DropItemUI>().PickupItem(this);
    }

    public void GetUnEaten()
    {
        //Debug.Log("Dropped;");
        IsEaten = false;
        GetComponent<MeshRenderer>().gameObject.SetActive(true);
        FindObjectOfType<DropItemUI>().DropItem(this);
    }
}
