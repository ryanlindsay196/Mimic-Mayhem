using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

    [SerializeField]
    InLevelHoardManager hoardManager;
    List<Eatable> eatenItems = new List<Eatable>();
    int uniqueItems = 0;

    [SerializeField]
    public GameObject EatenPanel;
    public Text eatTextPrefab;
    static Vector3 nextPosition;

    [SerializeField]
    List<Text> eatenItemsText;

	// Use this for initialization
	void Start () {
        //eatenItems = hoardManager.GetEaten();
        nextPosition = new Vector3(0, 22, 0);
        eatenItemsText = new List<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //eatenItemsText.Add(Instantiate(eatTextPrefab, nextPosition, eatTextPrefab.transform.rotation, EatenPanel.transform) as Text);
            Text newText = Instantiate(eatTextPrefab, EatenPanel.transform);
            eatenItemsText.Add(newText);
            Debug.Log(eatenItems.Count);
            for(int i = 0; i < eatenItems.Count; i++)
            {
                Vector3 tempPos = eatenItems[i].transform.position;
                tempPos.y += -(i * nextPosition.y);
                Debug.Log("FUCKIN ");
                eatenItems[i].transform.position = tempPos;
            }
            //nextPosition.y -= 22;
        }
	}

    void UpdateEatenUI()
    {
        for(int i = 0; i < eatenItems.Count; i++)
        {

        }
    }
}
