using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrapController : MonoBehaviour
{
    public Animator anim;

    public float timer = 0;
    public bool isUp = false;
    public int movementFlag = -1;
    
    public const float Damage = 10;
    public const double ContinuousDamageInterval = 1.5;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 3.0)
        {
            movementFlag = isUp ? -1 : 1;
            timer = 0;
            isUp = !isUp;
            anim.SetBool("isUp", isUp);
        }
    }

    private void FixedUpdate()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player_Physical"))
        {
            movementFlag = 0;
            StepOnTrap(other.transform.parent.GetComponent<PlayerController>());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player_Physical"))
        {
            LeaveTrap(other.transform.parent.GetComponent<PlayerController>());
            movementFlag = isUp ? 1 : -1;
        }
    }

    private void checkSpikeCnt(PlayerController player)
    {
        if (player.spikeCnt > player.stepSpikeCnt)
        {
            player.spikeCnt = player.stepSpikeCnt;
        }

        if (player.spikeCnt < 0)
        {
            player.spikeCnt = 0;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Debug.Log(movementFlag);
        if (other.CompareTag("Player_Physical"))
        {
            PlayerController player = other.transform.parent.GetComponent<PlayerController>();
            
            if (isUp && movementFlag == 1)
            {
                player.spikeCnt++;
                movementFlag = 0;
         
                if (player.spikeCnt == 1)
                {
                    player.TakeDamage((int)Damage);
                    player.StartSpikeTimer();
                }
            }
                 
            else if (!isUp && movementFlag == -1)
            {
                player.spikeCnt--;
                movementFlag = 0;
                movementFlag = 0;
         
                if (player.spikeCnt == 0)
                { 
                    player.StopSpikeTimer();
                }
            }
            //StayInTrap(other.GetComponent<PlayerController>());
            //test

            checkSpikeCnt(player);
        }
    }

    private void StepOnTrap(PlayerController player)
    {
        if (player.stepSpikeCnt++ == 0)
        {
            // spike trap deals damage to player
            if (isUp)
            {
                // player starts spikeDamageTimer for continuous damage from spike trap
                if (player.spikeCnt == 0)
                {
                    player.StartSpikeTimer();
                }
                
                player.spikeCnt++;
                player.TakeDamage((int)Damage);
            }
           
        }
        else
        {
            if (isUp)
            {
                player.spikeCnt++;
            }
        }

        checkSpikeCnt(player);
    }

    private void LeaveTrap(PlayerController player)
    {
        // player's spikeTrapCnt-- (if spikeTrapCnt == 0 then reset spikeDamageTimer)
        player.stepSpikeCnt--;
        if (isUp)
        {
            player.spikeCnt--;
        }
        if (player.spikeCnt == 0)
        {
            player.StopSpikeTimer();
        }

        checkSpikeCnt(player);
    }

    // private void StayInTrap(PlayerController player)
    // {
    //     
    // }
}
