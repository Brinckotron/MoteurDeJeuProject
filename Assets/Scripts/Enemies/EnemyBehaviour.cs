using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class EnemyBehaviour : MonoBehaviour
{

    [SerializeField]protected float speed, maxHealth, xpValue, sightRange, atkDelay, atkDamage, memorizePlayerPosDuration = 3f;
    public float AtkDamage
    {
        get { return atkDamage; }
        set { atkDamage = value; }
    }
    protected float CurrentHealth, AtkDelayTimer, HurtTimer, HurtDuration, MemorizePlayerPosTimer;
    protected bool IsHurt, IsDead;
    public bool isArenaMember;
    public bool isSleeping;
    [SerializeField] protected GameObject deathEffect, xpCrystal5, xpCrystal10, xpCrystal20, goldCoin, goldPouch, healthCrystal, staminaCrystal;
    protected GameObject Player;
    protected Collider2D PlayerCollider, MainCollider;
    protected Vector2? MemorizedPlayerPosition;
    protected Rigidbody2D Rb2D;
    [SerializeField] protected Transform castPos;
    [SerializeField] protected Animator anim;
    [SerializeField] protected AudioSource audioSource;
    protected LayerMask SightLayerMask = (1 << 9 | 1 << 10);
    public delegate void ImDead();

    public static event ImDead OnDeath;
    

    public virtual void TakeDamage(int dmg)
    {
        CurrentHealth -= dmg;
        if (CurrentHealth <= 0) Die();
        else IsHurt = true;
    }

    public virtual IEnumerator Die()
    {
        speed = 0;
        IsDead = true;
        if (isArenaMember && OnDeath != null) OnDeath();
        yield return new WaitForSeconds(3f);
        Debug.Log("yo");
        Instantiate(deathEffect, transform.position, transform.rotation);
        DeathDrop();
        Destroy(gameObject);
    }

    public virtual void DeathDrop()
    {
        
    }

    public virtual void FacePlayer()
    {
        Vector3 scale = transform.localScale;
        if (CanSeePlayer())
        {
            float x = transform.position.x < Player.transform.position.x ? 1f : -1f;
            transform.localScale = new Vector3(x, 1, 1);
        }
    }

    public virtual void MoveTowardsPlayer()
    {
    }

    public virtual void KeepPlayerPositionMemorized()
    {

        if (CanSeePlayer())
        {
            MemorizedPlayerPosition = Player.transform.position;
            MemorizePlayerPosTimer = memorizePlayerPosDuration;
        }
        else
        {
            if (MemorizePlayerPosTimer > 0)
            {
                MemorizePlayerPosTimer -= Time.deltaTime;
                if (MemorizePlayerPosTimer <= 0) MemorizedPlayerPosition = null;
            }
        }
    }

    public virtual void Stop()
    {
    }

    public virtual void Attack()
    {
    }

    public virtual bool CanSeePlayer()
    {
        var pos = transform.position;
        var playerPos = Player.transform.position;
        var playerPosHigh = new Vector3(playerPos.x, (playerPos.y + 0.15f), playerPos.z);
        var playerPosLow = new Vector3(playerPos.x, (playerPos.y - 0.15f), playerPos.z);
        var hitInfo1 = Physics2D.Raycast(pos, (playerPos - pos).normalized, sightRange, SightLayerMask);
        var hitInfo2 = Physics2D.Raycast(pos, (playerPosHigh - pos).normalized, sightRange, SightLayerMask);
        var hitInfo3 = Physics2D.Raycast(pos, (playerPosLow - pos).normalized, sightRange, SightLayerMask);
        if (!isSleeping)
            return (hitInfo1.collider == PlayerCollider || hitInfo2.collider == PlayerCollider ||
                    hitInfo3.collider == PlayerCollider);
        else return false;
    }
    
    public virtual void PlaySound(AudioSource source, AudioClip clip, float pitch = 1f, float volume = 0.2f)
    {
        var soundPoint = Instantiate(source, transform);
        soundPoint.clip = clip;
        soundPoint.volume = Mathf.Clamp(volume, 0f, 1f);
        soundPoint.pitch = Mathf.Clamp(pitch, 0f, 2f);
        soundPoint.Play();
        Destroy(soundPoint, clip.length);

    }
}
