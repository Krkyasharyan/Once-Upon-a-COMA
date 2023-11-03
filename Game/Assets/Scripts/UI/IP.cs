using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IP : MonoBehaviour
{
    ButtonController buttonController;
    TextMeshProUGUI text;

    void Start()
    {
        buttonController = this.transform.parent.parent.GetComponent<ButtonController>();
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        text.SetText("IP:" + ButtonController.getipadr());
    }
}
