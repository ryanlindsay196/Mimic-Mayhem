using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

    public GameObject titleScreen;
    public GameObject mainMenu;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        ChangeToMainMenu();
	}

    void ChangeToMainMenu()
    {
        if (titleScreen.activeInHierarchy == true && Input.anyKeyDown)
        {
            titleScreen.SetActive(false);
            mainMenu.SetActive(true);
        }

    }


    public void LoadTutorialLevel(int num)
    {
        SceneManager.LoadScene(num);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
