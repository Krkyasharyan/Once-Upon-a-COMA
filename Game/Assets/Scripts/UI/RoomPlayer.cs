using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoomPlayer : MonoBehaviour
{
    OnlineController onlineController;
    TextMeshProUGUI text;
    string ip;
    [SerializeField] int flag;
    // Start is called before the first frame update
    void Start()
    {
        onlineController = this.transform.parent.parent.GetComponent<OnlineController>();
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if(flag == 1) ip = onlineController.getp1();
        else ip = onlineController.getp2();
        text.SetText("PLAYER: " + ip);
    }
}
