using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField] int id;
    public ButtonController buttonController;
    public int now, max, ID;

    void Start()
    {
        now = 0;
        max = 2;
        ID = -1;
        buttonController = this.transform.parent.GetComponent<ButtonController>();
    }

    public void clicked()
    {
        if(now >= max || now == 0) return;
        buttonController.inputpassword(2, ID);
    }

    public int retnow()
    {
        return now;
    }

    public int retmax()
    {
        return max;
    }

    public int retid()
    {
        return ID;
    }

    void GetButtonText()
    {
        int index = (buttonController.page - 1) * 9 + id - 1;
        if(ButtonController.buttonList.Count <= index)
        {
            now = 0;
            ID = -1;
            return;
        }
        now = ButtonController.buttonList[index].num;
        ID = ButtonController.buttonList[index].id;
    }

    void Update()
    {
        GetButtonText();
    }
}