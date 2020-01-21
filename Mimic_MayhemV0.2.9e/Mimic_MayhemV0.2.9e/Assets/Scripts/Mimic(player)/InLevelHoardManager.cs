using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InLevelHoardManager : MonoBehaviour {
    [SerializeField]
    List<Eatable> eatenItems;
    [SerializeField]
    List<string> itemDescriptions;
    public int mimicControllerNumber;

    Eatable nextItemToEat;

    public struct ItemCollection
    {
        public string description;
        public bool hasBeenCollected;
    }

    public List<Eatable> GetEatenItems()
    {
        return eatenItems;
    }

    List<ItemCollection> itemEntriesInHoard;

    private void Start()
    {
        itemEntriesInHoard = new List<ItemCollection>();
        for (int i = 0; i < itemDescriptions.Count; i++)
        {
            ItemCollection tempItemCollection = new ItemCollection();
            tempItemCollection.description = itemDescriptions[i];
            tempItemCollection.hasBeenCollected = false;
            itemEntriesInHoard.Add(tempItemCollection);
        }
    }

    public void AddToHoard(Eatable itemToEat)
    {
        if (!itemToEat.IsEaten)
        {
            //itemToEat.GetEaten();
            //eatenItems.Add(itemToEat);

            nextItemToEat = itemToEat;
            Invoke("EatNextItem", 1);
        }
    }

    public void DropItems(Vector3 inPosition)
    {
        if (eatenItems.Count > 0)
        {
            int itemIndexToRemove = 0;
            int highestPointValue = 0;
            for (int i = 0; i < eatenItems.Count; i++)
            {
                if (eatenItems[i].GetPointValue > highestPointValue)
                {
                    highestPointValue = eatenItems[i].GetPointValue;
                    itemIndexToRemove = i;
                }
            }

            Debug.Log(eatenItems[itemIndexToRemove].GetItemName + " old position " + eatenItems[itemIndexToRemove].transform.position);
            eatenItems[itemIndexToRemove].enabled = true;
            eatenItems[itemIndexToRemove].GetUnEaten();
            eatenItems[itemIndexToRemove].transform.position = inPosition;
            Debug.Log(eatenItems[itemIndexToRemove].GetItemName + " new position " + eatenItems[itemIndexToRemove].transform.position);
            eatenItems.Remove(eatenItems[itemIndexToRemove]);
        }
    }

    public void EatNextItem()
    {
        if (nextItemToEat != null)
        {
            nextItemToEat.GetEaten();
            nextItemToEat.enabled = false;
            eatenItems.Add(nextItemToEat);
            nextItemToEat = null;
        }
    }

    public void UpdateHoard()
    {//Whenever this is called, update your collection of unique items for each new item acquired
        for(int i = 0; i < eatenItems.Count; i++)
        {//loop through eaten items to find the hoardID. If it encounters a new hoardID, set hasBeenCollected to true
            ItemCollection tempHoardEntry = itemEntriesInHoard[eatenItems[i].GetHoardID];
            tempHoardEntry.hasBeenCollected = true;
            itemEntriesInHoard[eatenItems[i].GetHoardID] = tempHoardEntry;
        }
    }

    public int GetCountOfItemType(Eatable.ItemType itemType)
    {//Make a reference to this object, and call this function when implementing pause menu UI
        int itemCountForType = 0;
        for(int i = 0; i < eatenItems.Count; i++)
        {
            if (eatenItems[i].GetComponent<Eatable>().itemType == itemType)
                itemCountForType++;
        }
        return itemCountForType;
    }
}
