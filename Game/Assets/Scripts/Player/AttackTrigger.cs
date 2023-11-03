using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class AttackTrigger : MonoBehaviour
{
    private Transform player;
    private Animator ani;


    void Start()
    {
        player = gameObject.transform.parent;
        ani = player.GetComponent<Animator>();
   
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("Enemy")) return;
        int damage = player.GetComponent<PlayerController>().ATK;
        collision.GetComponent<Character>().TakeDamage(damage);
    }
}