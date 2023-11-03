using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public GameObject doorUp, doorDown, doorLeft, doorRight;
    public bool up, down, left, right;
    public int doorNum;
    public Vector3 pointPos;
    
    // Start is called before the first frame update
    void Start()
    {
        doorLeft.SetActive(left);
        doorDown.SetActive(down);
        doorRight.SetActive(right);
        doorUp.SetActive(up);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateDoorNum()
    {
        if (up)
            doorNum++;
        if (down)
            doorNum++;
        if (left)
            doorNum++;
        if (right)
            doorNum++;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player_Physical"))
        {
            CameraController.CameraInstance.UpdateTarget(transform);
        }
    }
}
