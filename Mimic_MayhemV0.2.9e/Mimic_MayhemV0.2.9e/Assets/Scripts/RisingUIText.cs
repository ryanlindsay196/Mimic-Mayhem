using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RisingUIText : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        GetComponentInChildren<Text>().color = new Color(GetComponentInChildren<Text>().color.r,
                                                GetComponentInChildren<Text>().color.g,
                                                GetComponentInChildren<Text>().color.b,
                                                GetComponentInChildren<Text>().color.a - (Time.deltaTime / 2));

        transform.position += new Vector3(0f, 0.1f, 0f);

        if (GetComponentInChildren<Text>().color.a <= 0)
            Destroy(gameObject);
	}
}
