using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splash : MonoBehaviour
{
    [SerializeField] protected int damage;
    [SerializeField] protected float destroyTime = 0.5f;

    private void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 12)
        {
            DamageEffect(other.gameObject.GetComponentInParent<EnemyBehaviour>());
        }
        else if (other.gameObject.layer == 9)
        {
            other.gameObject.GetComponentInParent<PlayerController>().TakeDamage(damage/2, gameObject);
        }
        else if (other.gameObject.layer == 17)
        {
            other.gameObject.GetComponent<Breakable>().Break();
        }
    }

    public virtual void DamageEffect(EnemyBehaviour target)
    {
        target.TakeDamage(damage);
    }
}
