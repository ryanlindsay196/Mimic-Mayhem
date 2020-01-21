using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEMP_InputManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (ControllerManager.EatButton(0))
            Debug.Log(ControllerManager.MainJoystick(0));
        if (ControllerManager.EatButton(1))
            Debug.Log(ControllerManager.MainJoystick(1));
    }
}
