using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransformSelect : MonoBehaviour {

    [SerializeField]
    List<Button> transformSelections;//Buttons on screen you interact with to select your transformation

    //[SerializeField]
    public List<GameObject> playerPrefabs;

    public int playerTransformationIndex;//Which player transformation is active

    [SerializeField]
    Canvas transformUI;

    [SerializeField]
    float distanceFromCenter;

	// Use this for initialization
	void Start () {

        float angle = (Mathf.PI / 1) * 1;
        float angleOffset = Mathf.PI / 2;
        for (int i = 0; i < transformSelections.Count; i++)
        {
            transformSelections[i].GetComponent<RectTransform>().localPosition = new Vector3((distanceFromCenter * Mathf.Cos((angle * i) + angleOffset)),
                (distanceFromCenter * Mathf.Sin((angle * i) + angleOffset)),
                0.0f);
        }
	}

    public void SetEnabledPlayerPrefab(int newTransformationIndex)
    {//call this from the Onclick event in each button
        playerPrefabs[playerTransformationIndex].SetActive(false);
        playerPrefabs[newTransformationIndex].SetActive(true);

        playerPrefabs[newTransformationIndex].transform.position = playerPrefabs[playerTransformationIndex].transform.position;
        playerTransformationIndex = newTransformationIndex;

        transformUI.GetComponent<Canvas>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (ControllerManager.TransformButton(playerPrefabs[playerTransformationIndex].GetComponentInParent<InLevelHoardManager>().mimicControllerNumber))
        {
            Debug.Log(playerPrefabs[playerTransformationIndex].GetComponentInParent<InLevelHoardManager>().mimicControllerNumber);
            if (playerTransformationIndex == 0)
                transformUI.GetComponent<Canvas>().enabled = true;
            else
                SetEnabledPlayerPrefab(0);
        }
        else if (ControllerManager.TransormButtonReleased(playerPrefabs[playerTransformationIndex].GetComponentInParent<InLevelHoardManager>().mimicControllerNumber))
            transformUI.GetComponent<Canvas>().enabled = false;
        else if (transformUI.GetComponent<Canvas>().enabled)
        {
            if (ControllerManager.SelectTransformation1(playerPrefabs[playerTransformationIndex].GetComponentInParent<InLevelHoardManager>().mimicControllerNumber) >= 1f)
            {
                SetEnabledPlayerPrefab(1);
                Debug.Log("greater than");
                transformUI.GetComponent<Canvas>().enabled = false;
            }
            else if (ControllerManager.SelectTransformation1(playerPrefabs[playerTransformationIndex].GetComponentInParent<InLevelHoardManager>().mimicControllerNumber) <= -0.08f)
            {
                SetEnabledPlayerPrefab(2);
                Debug.Log("less than");
                transformUI.GetComponent<Canvas>().enabled = false;
            }
        }
    }
}
