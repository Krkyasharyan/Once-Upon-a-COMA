using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoomText : MonoBehaviour
{
    [SerializeField] OnlineController onlineController;
    TextMeshProUGUI text;
    string roomid;

    // Start is called before the first frame update
    void Start()
    {
        onlineController = this.transform.parent.parent.GetComponent<OnlineController>();
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        roomid = onlineController.getroomid();
        text.SetText("RoomID: " + roomid);
    }
}
