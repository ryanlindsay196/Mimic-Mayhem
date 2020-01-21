using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickupable : MonoBehaviour {

    // Use this for initialization
    public ItemBase InventoryItem;
    public PlayerMovement playerMovement;
    public Eatable eatable;
    GameObject player;
    bool amIEaten = false;

    // Use this for initialization

    void Start()
    {
        

        player = GameObject.FindWithTag("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
        eatable = GetComponent<Eatable>();

         
    }

    // Update is called once per frame
    void Update()
    {
        if (eatable != null)
        {
            amIEaten = eatable.IsEaten;
        }

        if(amIEaten){
            Debug.Log("eaten");
            playerMovement.ReceiveItem(InventoryItem);
            //player.ReceiveItem(Instantiate(InventoryItem));
            Destroy(gameObject);
        }
    }
	}