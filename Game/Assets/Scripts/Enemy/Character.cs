using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public int maxHP;
    public int curHP;
    public int ATK;
    public bool DIE;
    Animator animator;

    void Start() {
        maxHP = 50;
        curHP = maxHP;
        ATK = 5;
        DIE = false;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage) {
        if(curHP > 0)
        {
            curHP -= damage;
            if(curHP <= 0)
            {
                DIE = true;
                animator.SetFloat("DIE",1);

                Collider2D collider = GetComponent<Collider2D>();
                if (collider != null)
                {
                    collider.enabled = false;
                }

                Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
                if (rigidbody != null)
                {
                    rigidbody.simulated = false;
                }
                
                Destroy(gameObject, 3.0f);
            }
            animator.SetTrigger("Hitted");
        }
        else return;
    }
}
