// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class PlayerController : MonoBehaviour
// {
//     public float speed = (float)10;
//
//     private Rigidbody2D rb;
//
//     private Vector3 p;
//
//     private static float moveX, moveY;
//     void Start()
//     {
//         rb = GetComponent<Rigidbody2D>();
//     }
//
//     void Update()
//     {
//         moveX=0;
//         moveY=0;
//         float amoveX = Input.GetAxisRaw("Horizontal");
//         float amoveY = Input.GetAxisRaw("Vertical");
//         if(amoveX!=0||amoveY!=0)
//             NetController.sendMove((int)amoveX,(int)amoveY);
//         // p = transform.position;
//         // p.x += moveX * speed * Time.deltaTime;
//         // p.y += moveY * speed * Time.deltaTime;
//         // transform.position = p;
//
//         int faceDir = (int)transform.localScale.x;
//
//         if(amoveX > 0)
//         {
//             faceDir = 1;
//         }
//         else if(amoveX < 0)
//         {
//             faceDir = -1;
//         }
//         transform.localScale = new Vector3(faceDir, 1, 1);
//     }
//
//     private void FixedUpdate()
//     {
//         rb.MovePosition((rb.position + speed * Time.fixedDeltaTime * new Vector2(moveX, moveY)));
//     }
//     public static void PlayerMove(int x,int y)
//     {
//         moveX=(float) x;
//         moveY=(float)y;
//     }
// }