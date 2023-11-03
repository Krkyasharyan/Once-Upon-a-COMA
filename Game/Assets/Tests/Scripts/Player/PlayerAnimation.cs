using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private PlayerController player;

    private void Awake() {
        animator = GetComponent<Animator>();
        player = GetComponent<PlayerController>();
    }

    private void Update() {
        SetAnimation();
    }

    private void SetAnimation() {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        if(moveX != 0 || moveY != 0) moveX = 1;
        animator.SetFloat("velocityX", moveX);

        if(Input.GetKeyDown(KeyCode.J))
        {
            animator.SetTrigger("attackKey");
            animator.SetBool("isAttack", true);
        }

        if(Input.GetKeyDown(KeyCode.K) && player.cdTimer >= player.cdTime)
        {
            animator.SetTrigger("dodgeKey");
            player.cdTimer = 0;
            player.iTime = 0.5f;
            player.speedX = 20;
        }

        if(player.isDead())
        {
            animator.SetBool("isDie", true);
        }
    }
}
