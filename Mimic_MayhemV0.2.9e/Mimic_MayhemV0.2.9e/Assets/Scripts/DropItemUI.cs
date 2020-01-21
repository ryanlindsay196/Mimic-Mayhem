using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropItemUI : MonoBehaviour {

    public GameObject textPrefab;
    [SerializeField]
    Vector3 spawnPosition;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PickupItem(Eatable inEatable)
    {
        Debug.Log("Pickup");
        textPrefab.GetComponent<Text>().text = " \tEaten " + inEatable.GetItemName + "\t" + inEatable.GetPointValue + " points!";
        Instantiate(textPrefab, transform);
    }
    public void DropItem(Eatable inEatable)
    {
        Debug.Log("Drop");
        textPrefab.GetComponent<Text>().text = " \tDropped " + inEatable.GetItemName + "\t" + inEatable.GetPointValue + " points!";
        Instantiate(textPrefab, transform);
    }
}
