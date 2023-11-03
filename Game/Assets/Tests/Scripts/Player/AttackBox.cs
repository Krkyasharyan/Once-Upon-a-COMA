using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBox : MonoBehaviour
{
    private Animator ani;
    private AnimatorStateInfo state;
    private Transform attack;
    private Transform attack1;
    private Transform attack2;


    void Start()
    {
        ani = gameObject.GetComponent<Animator>();
        attack = transform.Find("attack");
        attack1 = transform.Find("attack1");
        attack2 = transform.Find("attack2");
        attack.gameObject.SetActive(false);
        attack1.gameObject.SetActive(false);
        attack2.gameObject.SetActive(false);
    }

    
    void Update()
    {
        
    }

    public void Attackenemy()
    {
        state = ani.GetCurrentAnimatorStateInfo(0);
        if(state.IsName("player1_attack"))
        {
            attack.gameObject.SetActive(true);
        }
        else if(state.IsName("player1_attack1"))
        {
            attack1.gameObject.SetActive(true);
        }
        else if(state.IsName("player1_attack2"))
        {
            attack2.gameObject.SetActive(true);
        }
        Invoke("endAttackenemy", 0.05f);
    }

    public void endAttackenemy()
    {
        state = ani.GetCurrentAnimatorStateInfo(0);
 
        if(state.IsName("player1_attack"))
        {
            attack.gameObject.SetActive(false);
        }
        else if(state.IsName("player1_attack1"))
        {
            attack1.gameObject.SetActive(false);
        }
        else if(state.IsName("player1_attack2"))
        {
            attack2.gameObject.SetActive(false);
        }
    }
}
