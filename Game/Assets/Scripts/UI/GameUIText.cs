using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUIText : MonoBehaviour
{
    public TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void setText(string content)
    {
        text.SetText(content);
    }
    void Update()
    {
        
    }
}
