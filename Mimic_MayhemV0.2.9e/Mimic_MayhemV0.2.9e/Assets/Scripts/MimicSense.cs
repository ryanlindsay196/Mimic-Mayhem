using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MimicSense : MonoBehaviour {
    float timer;
    [SerializeField]
    float maxTime;

	// Use this for initialization
	void Start () {
		
	}

    public void SetText(string newText)
    {
        GetComponent<Text>().text = newText;
        timer = 0;
    }
	
	// Update is called once per frame
	void Update () {
		if(timer <= maxTime)
        {
            timer += Time.deltaTime;
        }

        GetComponent<Text>().color = new Color(GetComponent<Text>().color.r,
                                                GetComponent<Text>().color.g,
                                                GetComponent<Text>().color.b,
                                                (maxTime - timer) / maxTime);

        //transform.position = new Vector3(transform.position.x, 200 + (timer - (maxTime / 2) * 20), transform.position.z);
	}
}
