using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private PlayerController player;

    public bool attackFlag = false;
    public bool dodgeFlag = false;

    private void Awake() {
        animator = GetComponent<Animator>();
        player = GetComponent<PlayerController>();
    }

    private void Update() {
        if(Time.timeScale!=0)
            SetAnimation();
        if (attackFlag)
        {
            Debug.Log("Attack?");
            Attack();
            attackFlag = false;
        }

        if (dodgeFlag)
        {
            Dodge();
            dodgeFlag = false;
        }
    }

    private void SetAnimation() {
        //float moveX = Input.GetAxisRaw("Horizontal");
        //float moveY = Input.GetAxisRaw("Vertical");
        float moveX = player.moveX;
        float moveY = player.moveY;
        if(moveX != 0 || moveY != 0) moveX = 1;
        animator.SetFloat("velocityX", moveX);

        if(Input.GetKeyDown(KeyCode.J))
        {
            if(!player.connect) //NetController.sendAttack(1,0);
            {
                // animator.SetTrigger("attackKey");
                // animator.SetBool("isAttack", true);
                Attack();
            }
        }

        if(Input.GetKeyDown(KeyCode.K) && player.cdTimer >= player.cdTime)
        {
            if(!player.connect) //NetController.sendAttack(0,1);
            {
                // animator.SetTrigger("dodgeKey");
                // player.cdTimer = 0;
                // player.iTime = 0.5f;
                // player.speedX = 20;
                Dodge();
            }
        }

        if(player.isDie())
        {
            animator.SetBool("isDie", true);
        }
    }

    private void SwitchToUI()
    {
        SceneManager.LoadScene("UIScene");
    }

    public void Attack()
    {
        Debug.Log("Attack!");
        animator.SetTrigger("attackKey");
        animator.SetBool("isAttack", true);
    }

    public void Dodge()
    {
        animator.SetTrigger("dodgeKey");
        player.cdTimer = 0;
        player.iTime = 0.5f;
        player.speedX = 20;
    }
}
