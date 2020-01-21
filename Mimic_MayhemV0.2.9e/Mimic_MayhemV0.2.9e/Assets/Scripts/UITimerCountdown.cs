using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UITimerCountdown : MonoBehaviour {
    [SerializeField]
    float matchStartTimeInSeconds;

    [SerializeField]
    Text playerWinAnnounceText;
    

    [SerializeField]
    Text winningPlayerText;
    [SerializeField]
    Text p1TextBox;
    [SerializeField]
    Text p1TotalScoreTextBox;
    [SerializeField]
    Text p2TotalScoreTextBox;
    [SerializeField]
    Text p2TextBox;
    [SerializeField]
    InLevelHoardManager player1Hoard;//attach player_parent 1
    [SerializeField]
    InLevelHoardManager player2Hoard;//attach player_parent 2

    bool finishedTallying;
    float tallyFuncInput;

    [SerializeField]
    Text textBoxPrefab;

    int p1Points;
    int p2Points;

    [SerializeField]
    List<Text> p1ItemTextBoxes, p2ItemTextBoxes;

    float currentTimeInSeconds;

    int finalScoreTallyItemIndex;//When tallying final score, this increments as text gets added to the score tally text box
    bool isTallying;
	// Use this for initialization
	void Start () {
        currentTimeInSeconds = matchStartTimeInSeconds;
        p1TextBox.text = "";
        p2TextBox.text = "";
        isTallying = false;
        finishedTallying = false;
	}
	
    void UpdateItems(InLevelHoardManager playerHoard, int i, List<Text> playerItemTextBoxes, ref int playerPoints, ref Text totalScoreTextBox)
    {

        if (playerItemTextBoxes[i].GetComponent<RectTransform>().anchoredPosition.y >= -75)
        {//if above maximum height
            playerItemTextBoxes[i].GetComponent<RectTransform>().anchoredPosition += new Vector2(2, 0);
            //Debug.Log("LVL 1");
            playerItemTextBoxes[i].color -= new Color(0, 0, 0, 0.02f);
            playerItemTextBoxes[i].transform.localScale = Vector2.Lerp(playerItemTextBoxes[i].transform.localScale, new Vector2(1f, 0f), 0.4f);

            if(i >= playerItemTextBoxes.Count - 1 || i >= playerHoard.GetEatenItems().Count - 1)
            {
                finishedTallying = true;
            }
        }
        else if (playerItemTextBoxes[i].GetComponent<RectTransform>().anchoredPosition.y >= -95)
        {//if not greater than X but greater than Y
            playerPoints += playerHoard.GetEatenItems()[i].GetPointValue;

            if(playerHoard.GetEatenItems()[i].GetPointValue != 0)
            {
                totalScoreTextBox.transform.eulerAngles = new Vector3(0, 0, 35);
            }
            else
            {
                totalScoreTextBox.transform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(totalScoreTextBox.transform.eulerAngles.z, 0, 0.5f));
            }
            playerHoard.GetEatenItems()[i].SetPointValue = 0;
            totalScoreTextBox.text = "Total Score: " + playerPoints;
            finishedTallying = false;
            //Debug.Log("LVL 2");
            playerItemTextBoxes[i].GetComponent<RectTransform>().anchoredPosition += new Vector2(0, 2);
            //playerItemTextBoxes[i].GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(playerItemTextBoxes[i].GetComponent<RectTransform>().sizeDelta, new Vector2(10.5f, 1.5f), 0.2f);
            playerItemTextBoxes[i].transform.localScale = Vector2.Lerp(playerItemTextBoxes[i].transform.localScale, new Vector2(1.2f, 1.2f), 0.4f);
        }
        else
        {//move up
            finishedTallying = false;
            //Debug.Log("LVL 3");
            playerItemTextBoxes[i].GetComponent<RectTransform>().anchoredPosition += new Vector2(0, 0.8f);
        }
    }

	// Update is called once per frame
	void Update () {
        string tempSecondsString = Mathf.RoundToInt((currentTimeInSeconds % 60)-1).ToString();
        if (tempSecondsString.Length <= 1)
            tempSecondsString = "0" + tempSecondsString;
        if (!isTallying)
        {
            GetComponent<Text>().text = Mathf.Floor(currentTimeInSeconds / 60) + ":" + tempSecondsString;
        }
        else if (isTallying)
        {
            FindObjectOfType<TEMPItemCountUI>().GetComponent<Image>().enabled = true;

            for (int i = 0; i < player1Hoard.GetEatenItems().Count || i < player2Hoard.GetEatenItems().Count; i++)
            {
                if (i < p1ItemTextBoxes.Count)
                {
                    UpdateItems(player1Hoard, i, p1ItemTextBoxes, ref p1Points, ref p1TotalScoreTextBox);
                }
                if (i < p2ItemTextBoxes.Count)
                {
                    UpdateItems(player2Hoard, i, p2ItemTextBoxes, ref p2Points, ref p2TotalScoreTextBox);
                }
            }

            if(finishedTallying)
            {
                tallyFuncInput += Time.deltaTime * 50;

                playerWinAnnounceText.GetComponent<RectTransform>().anchoredPosition = new Vector2(playerWinAnnounceText.GetComponent<RectTransform>().anchoredPosition.x, 15 * Mathf.Abs(Mathf.Abs(Mathf.Sin(tallyFuncInput))) / Mathf.Pow(tallyFuncInput, 2));

                if(p1Points > p2Points)
                {
                    playerWinAnnounceText.text = "Player 1 Wins!";
                }
                else if(p2Points > p1Points)
                {
                    playerWinAnnounceText.text = "Player 2 Wins!";
                }
                else
                {
                    playerWinAnnounceText.text = "Tie!";
                }
            }
        }
        currentTimeInSeconds -= Time.deltaTime;
        currentTimeInSeconds = Mathf.Clamp(currentTimeInSeconds, 0, float.MaxValue);
        if (currentTimeInSeconds <= 0 && !isTallying)
        {
            //InvokeRepeating("UpdateScoreTallyTextBoxes", 0.1f, 1f);
            isTallying = true;
            for(int i = 0; i < player1Hoard.GetEatenItems().Count || i < player2Hoard.GetEatenItems().Count; i++)
            {
                if (i < player1Hoard.GetEatenItems().Count)
                {
                    Text p1Item = Instantiate(textBoxPrefab, new Vector3(250, -i * 25 + 350), Quaternion.identity, gameObject.transform);
                    p1TotalScoreTextBox.transform.position = new Vector3(250, p1TotalScoreTextBox.transform.position.y);
                    p1Item.text = player1Hoard.GetEatenItems()[i].GetItemName + ": " + player1Hoard.GetEatenItems()[i].GetPointValue;
                    p1ItemTextBoxes.Add(p1Item);
                }
                if (i < player2Hoard.GetEatenItems().Count)
                {
                    Text p2Item = Instantiate(textBoxPrefab, new Vector3(1250, -i * 25 + 350), Quaternion.identity, gameObject.transform);
                    p2TotalScoreTextBox.transform.position = new Vector3(1250, p2TotalScoreTextBox.transform.position.y);
                    p2Item.text = player2Hoard.GetEatenItems()[i].GetItemName + ": " + player2Hoard.GetEatenItems()[i].GetPointValue;
                    p2ItemTextBoxes.Add(p2Item);
                }
            }
            GetComponent<Text>().text = "Times Up!";

            VillagerMain[] villagers = FindObjectsOfType<VillagerMain>();
            foreach (VillagerMain villager in villagers)
                villager.endOfGame = true;
        }
        else if(currentTimeInSeconds <= 0)
        {
            if (ControllerManager.EatButton(1) || ControllerManager.EatButton(0))
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
	}

    void UpdateScoreTallyTextBoxes()
    {//Runs after x time repeatedly until finalScoreTallyItemIndex = the lenght of the longest players item list
        try
        {
            p1TextBox.text += player1Hoard.GetEatenItems()[finalScoreTallyItemIndex].GetItemName + ":\t" + player1Hoard.GetEatenItems()[finalScoreTallyItemIndex].GetPointValue + "\n";
            p1Points += player1Hoard.GetEatenItems()[finalScoreTallyItemIndex].GetPointValue;
            p1TotalScoreTextBox.text = "Total Score: " + p1Points;
        }
        catch
        {
            Debug.Log("Error adding p1 points");
        }
        try
        {
            p2TextBox.text += player2Hoard.GetEatenItems()[finalScoreTallyItemIndex].GetItemName + ":\t" + player2Hoard.GetEatenItems()[finalScoreTallyItemIndex].GetPointValue + "\n";
            p2Points += player2Hoard.GetEatenItems()[finalScoreTallyItemIndex].GetPointValue;
            p2TotalScoreTextBox.text = "Total Score: " + p2Points;
        }
        catch
        {
            Debug.Log("Error adding p2 points");
        }
        //Debug.Log("Update tally");
        if (finalScoreTallyItemIndex > player1Hoard.GetEatenItems().Count && finalScoreTallyItemIndex > player2Hoard.GetEatenItems().Count)
        {
            CancelInvoke();
            if (p1Points > p2Points)
                winningPlayerText.text = "Player 1 Wins!";
            else if (p2Points > p1Points)
                winningPlayerText.text = "Player 2 Wins!";
            else
                winningPlayerText.text = "Draw";

            winningPlayerText.text += "\nPress A for Rematch";

            Debug.Log("Tally finished");
        }
        finalScoreTallyItemIndex++;
    }


}
