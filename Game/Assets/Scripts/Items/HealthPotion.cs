using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class HealthPotion : MonoBehaviour
{

    public int HealthBuff = 25;
    private Slot slot;
    private PlayerController controller;

    private void Start()
    {
        slot = transform.parent.GetComponent<Slot>();
        controller = slot.inventory.GetComponent<PlayerController>();
    }

    public void Use()
    {
        //PlayerController controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        controller.curHP = Mathf.Clamp(controller.curHP+HealthBuff, 0, 100);
        controller.healthbar.SetHealth(controller.curHP);
        if (controller.player_id != 0)
        {
            WebController.sendHpUp(controller.curHP);
        }
        slot.DropItem();
    }
}
