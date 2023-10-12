using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinAttack : MonoBehaviour
{
    private GoblinController Goblin;
    private int damage;

    private void Awake()
    {
        Goblin = gameObject.GetComponentInParent<GoblinController>();
        damage = (int)Goblin.atkDamage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 9)
        {
            Debug.Log("Hit!");
            other.gameObject.GetComponentInParent<PlayerController>().Instance.TakeDamage(damage, Goblin.gameObject);
        }
    }
}
