using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonText : MonoBehaviour
{
    [SerializeField] string type;
    public TextMeshProUGUI text;
    public Button button;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        button = this.transform.parent.GetComponent<Button>();
    }

    void Print()
    {
        if(type == "ID")
        {
            if(button.retid() == -1) text.SetText("RoomID: #");
            else text.SetText("RoomID: " + button.retid());
        }
        else
        {
            text.SetText(button.retnow().ToString() + "/" + button.retmax().ToString());
        }
    }

    void Update()
    {
        Print();
    }
}
