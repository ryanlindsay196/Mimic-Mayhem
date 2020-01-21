using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectOnInput : MonoBehaviour {

    public EventSystem eventSystem;
    public GameObject selectedObj;
    public Button backButton;

    private bool buttonSelected = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetAxisRaw("Vertical") != 0 && buttonSelected == false)
        {
            eventSystem.SetSelectedGameObject(selectedObj);
            buttonSelected = true;
        }
        else if (Input.GetButtonDown("Cancel"))
        {
            backButton.onClick.Invoke();
        }
	}

    private void OnDisable()
    {
        buttonSelected = false;
    }
}
