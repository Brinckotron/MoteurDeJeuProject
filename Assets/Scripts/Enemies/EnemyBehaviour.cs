using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBehaviour : MonoBehaviour
{

    [SerializeField]protected float speed, maxHealth, xpValue, sightRange;
    protected float CurrentHealth;
    [SerializeField] protected GameObject deathEffect;
    protected GameObject Player;
    [SerializeField] protected LayerMask sightLayerMask;
    
    public virtual void TakeDamage(int dmg)
    {
        CurrentHealth -= dmg;
        if (CurrentHealth <= 0) Die();
    }

    public virtual IEnumerator Die()
    {
        speed = 0;
        yield return new WaitForSeconds(3f);
        Instantiate(deathEffect, transform.position, transform.rotation);
        DeathDrop();
        Destroy(gameObject);
    }

    public virtual void DeathDrop()
    {
        
    }

    public virtual bool CanSeePlayer()
    {
        var hitInfo = Physics2D.Raycast(transform.position, (Player.transform.position - transform.position).normalized, sightRange, sightLayerMask);
        return hitInfo.collider == Player.GetComponentInChildren<CapsuleCollider2D>();
    }
}
