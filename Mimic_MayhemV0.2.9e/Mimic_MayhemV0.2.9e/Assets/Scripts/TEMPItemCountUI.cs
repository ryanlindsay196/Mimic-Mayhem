using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TEMPItemCountUI : MonoBehaviour {
    [SerializeField]
    List<Text> textFields;
    [SerializeField]
    InLevelHoardManager hoardManager;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < textFields.Count; i++)
        {
            Eatable.ItemType tempItemType = (Eatable.ItemType)i;
            textFields[i].text = tempItemType.ToString() + ": " + hoardManager.GetCountOfItemType(tempItemType);
        }
	}
}
