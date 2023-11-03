using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuiltController : MonoBehaviour
{
    public GameObject dialogueUI;
    public GameObject button_space;

    private int buttonCnt = 0;
    private int player_id = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        button_space.SetActive(false);
        dialogueUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // if (buttonCnt > 0 && dialogueUI.activeSelf && Input.GetKeyDown(KeyCode.Space))
        // {
        //     Debug.Log(buttonCnt);
        //     dialogueUI.SetActive(false);
        // }        
        
        if (button_space.activeSelf && Input.GetKeyDown(KeyCode.Space) && buttonCnt == 0 && player_id == WebController.whoamI)
        {
            dialogueUI.SetActive(true);
            buttonCnt++;  
        }

        // if (buttonCnt > 0 && button_space.activeSelf)
        // {
        //     button_space.SetActive(false);
        // }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player_Physical") && buttonCnt == 0)
        {
            player_id = other.GetComponentInParent<PlayerController>().player_id;
            button_space.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player_Physical"))
        {
            button_space.SetActive(false);
        }
    }
}
