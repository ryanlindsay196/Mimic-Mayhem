using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerUIManager : MonoBehaviour {

    public List<GameObject> uiSprites;
    [SerializeField]
    int uiSpriteIndex;

    [SerializeField]
    float maxAnimationTime;
    float currentAnimationTime;


	// Use this for initialization
	void Start () {
		
	}

    private void FixedUpdate()
    {
        if (currentAnimationTime < maxAnimationTime)
            currentAnimationTime += Time.deltaTime * 3;

        
        if (uiSpriteIndex > -1)
        {//if villager has sprite above it, rotate sprite towards camera
            uiSprites[uiSpriteIndex].transform.localScale = new Vector3(-(1 + Mathf.Sin(currentAnimationTime * 3)), 1 + Mathf.Sin((currentAnimationTime * 3) - Mathf.PI)) / 2;

            Vector3 targetDir = Camera.main.transform.position - transform.position;
            uiSprites[uiSpriteIndex].transform.rotation = Quaternion.LookRotation(Camera.main.transform.position);

            if(uiSpriteIndex == 2)//flee index
            {//make it shake
                uiSprites[uiSpriteIndex].transform.eulerAngles = new Vector3(uiSprites[uiSpriteIndex].transform.eulerAngles.x, uiSprites[uiSpriteIndex].transform.eulerAngles.y, uiSprites[uiSpriteIndex].transform.eulerAngles.z + (Mathf.Sin(currentAnimationTime * 3) * 205));
            }
        }
    }

    public void ChangeUISprite(int newUISpriteIndex)
    {
        currentAnimationTime = 0;//reset animation timer
        if(uiSpriteIndex > -1)
            uiSprites[uiSpriteIndex].GetComponent<SpriteRenderer>().enabled = false;
        if(newUISpriteIndex > -1)
            uiSprites[newUISpriteIndex].GetComponent<SpriteRenderer>().enabled = true;
        uiSpriteIndex = newUISpriteIndex;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
