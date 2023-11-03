using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GptMatController : MonoBehaviour
{
    public GameObject dialogueUI;
    public GameObject button_space;

    private int buttonCnt = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        button_space.SetActive(false);
        dialogueUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (button_space.activeSelf && Input.GetKeyDown(KeyCode.Space) && buttonCnt == 0)
        {
            dialogueUI.SetActive(true);
            buttonCnt++;  
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player_Physical") && buttonCnt == 0)
        {
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