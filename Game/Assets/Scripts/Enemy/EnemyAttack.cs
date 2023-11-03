using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int damage;

    private void OnTriggerStay2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;
        damage = this.GetComponent<Character>().ATK;
        other.GetComponent<PlayerController>().TakeDamage(damage);
    }
}
