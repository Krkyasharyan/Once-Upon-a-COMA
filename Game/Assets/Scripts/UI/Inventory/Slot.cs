using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public Inventory inventory;
    public int slotID;

    private void Start()
    {
        //inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }

    private void Update()
    {
        if(transform.childCount <= 1)
        {
            inventory.isFull[slotID] = false;
            inventory.itemIdList[slotID] = 0;
            //inventory.SaveItems();
        }
    }

    public void DropItem()
    {
        foreach (Transform child in transform)
        {
            //child.GetComponent<Spawn>().SpawnDroppedItem();
            if (child.name == "Cross") continue;
            GameObject.Destroy(child.gameObject); 
            
        }
    }
}