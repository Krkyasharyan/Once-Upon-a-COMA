using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    public Text textUI;
    public Image avatar;
    public TextAsset textFile;
    public int index;
    public Sprite avatar_player, avatar_narrator, avatar_gpt;

    private List<string> textList = new List<string>();
    
    // Start is called before the first frame update
    void Awake()
    {
        GetText(textFile);
        index = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        if (Input.anyKeyDown)
        {
            Debug.Log(index);
            //Debug.Log("Space is down");
            if (index == textList.Count)
            {
                gameObject.SetActive(false);
                index = 0;
                return;
            }

            Debug.Log(textList[index]);
            var speaker = GetSpeaker(textList[index]);
            if (speaker == "Player")
            {
                Debug.Log("Here we go");
                avatar.sprite = avatar_player;
                index++;
                //return;
            }
            else if (speaker == "Narrator")
            {
                avatar.sprite = avatar_narrator;
                index++;
                //return;
            }
            else if (speaker == "Gpt")
            {
                avatar.sprite = avatar_gpt;
                index++;
            }
            textUI.text = textList[index];
            index++;
        }
        // else if (Input.GetKeyDown(KeyCode.K))
        // {
        //     Debug.Log("K is down");
        // }
    }

    private void OnEnable()
    {
        Debug.Log(index);
        Debug.Log(textList[index]);
        var speaker = GetSpeaker(textList[index]);
        Debug.Log("Speaker is " + speaker + "!");
        if (speaker == "Player")
        {
            avatar.sprite = avatar_player;
            index++;
            return;
        }
        // else if (speaker == "Narrator")
        // {
        //     avatar.sprite = avatar_narrator;
        //     index++;
        //     return;
        // }
        textUI.text = textList[index];
        index++;
    }

    void GetText(TextAsset file)
    {
        textList.Clear();
        index = 0;

        var lines = file.text.Split('\n');

        foreach (var line in lines)
        {
            Debug.Log(line);
            textList.Add(line);
        }
    }

    String GetSpeaker(String input)
    {
        if (input.Length < 3 || input[0] != '#')
        {
            //throw new ArgumentException("Invalid input");
            return "";
        }

        int firstHashIndex = input.IndexOf('#');
        int secondHashIndex = input.LastIndexOf('#');

        if (firstHashIndex == secondHashIndex)
        {
            //throw new ArgumentException("Invalid input");
            return "";
        }

        return input.Substring(firstHashIndex + 1, secondHashIndex - firstHashIndex - 1);
    }
}
