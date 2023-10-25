using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinAttack : MonoBehaviour
{
    private GoblinController _goblin;
    private int _damage;

    private void Awake()
    {
        _goblin = gameObject.GetComponentInParent<GoblinController>();
        _damage = (int)_goblin.AtkDamage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 9)
        {
            other.gameObject.GetComponentInParent<PlayerController>().TakeDamage(_damage, _goblin.gameObject);
        }
    }
}
