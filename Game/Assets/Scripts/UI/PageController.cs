using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PageController : MonoBehaviour
{
    [SerializeField] ButtonController buttonController;
    TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        string s = "";
        s = buttonController.page.ToString() + "/" + buttonController.getmaxpage().ToString();
        text.SetText(s);
    }
}