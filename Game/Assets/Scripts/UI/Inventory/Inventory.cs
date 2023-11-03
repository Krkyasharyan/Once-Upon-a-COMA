using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject[] slots;
    public bool[] isFull;
    public int[] itemIdList;

    private int player_id;

    [Header("ItemButtons")] public GameObject healthPotionButton;

    // Start is called before the first frame update
    void Start()
    {
        player_id = GetComponent<PlayerController>().player_id;
        LoadItems();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveItems()
    {
        int idMask = 0;
        foreach (var itemId in itemIdList)
        {
            idMask *= 10;
            idMask += itemId;
        }

        if (player_id == 0)
        {
            PlayerPrefs.SetInt("items", idMask);
        }
        else if (player_id == 1)
        {
            Debug.Log("P1's items");
            Debug.Log(idMask);
            PlayerPrefs.SetInt("P1items", idMask);
        }
        else if (player_id == 2)
        {
            Debug.Log("P2's items");
            Debug.Log(idMask);
            PlayerPrefs.SetInt("P2items", idMask);
        }
        
        PlayerPrefs.Save();
        //Debug.Log(idMask);
    }

    public void LoadItems()
    {
        string key = "";
        switch (player_id)
        {
            case 0:
                key = "items";
                break;
            case 1:
                key = "P1items";
                break;
            case 2:
                key = "P2items";
                break;
        }
        Debug.Log(key);
        int idMask = PlayerPrefs.GetInt(key, 0);
        for (int i = 3; i >= 0; i--)
        {
            itemIdList[i] = idMask % 10;
            idMask /= 10;
        }

        for (int i = 0; i < 4; i++)
        {
            int itemId = itemIdList[i];
            switch (itemId)
            {
                case 0:
                    isFull[i] = false;
                    break;
                case 1:
                    isFull[i] = true;
                    Instantiate(healthPotionButton, slots[i].transform, false);
                    break;
            }
        }
    }
}