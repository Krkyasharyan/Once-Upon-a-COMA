using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] GameObject StartUI;
    [SerializeField] GameObject RoomListUI;
    [SerializeField] GameObject OnlineUI;
    // Start is called before the first frame update
    void Start()
    {
        UIStart();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UIMultiplayer()
    {
        StartUI.SetActive(false);
        RoomListUI.SetActive(true);
        OnlineUI.SetActive(false);
    }

    public void UIStart()
    {
        StartUI.SetActive(true);
        RoomListUI.SetActive(false);
        OnlineUI.SetActive(false);
    }

    public void UIOnline()
    {
        StartUI.SetActive(false);
        RoomListUI.SetActive(false);
        OnlineUI.SetActive(true);
    }
}
