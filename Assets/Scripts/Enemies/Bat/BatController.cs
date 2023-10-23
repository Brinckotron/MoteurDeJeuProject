using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatController : FlyerEnemy
{
    private string _currentState;
    [SerializeField] private float isNearCeilingRayDistance = 0.11f;
    
    
    private void Start()
    {
        Player = GameObject.FindWithTag("Player");
        PlayerCollider = Player.GetComponentInChildren<CapsuleCollider2D>();
        Rb2D = GetComponent<Rigidbody2D>();
        MainCollider = GetComponent<CircleCollider2D>();
        CurrentHealth = maxHealth;
    }

    private void FixedUpdate()
    {
        GoPerch();
    }

    private void GoPerch()
    {
        if (!CanSeePlayer() && MemorizedPlayerPosition == null)
        {
            if (IsNearCeiling())
            {
                Rb2D.velocity = Vector2.zero;
                ChangeAnimationState("Perched");
            }
            else
            {
                ChangeAnimationState("Fly");
                Rb2D.velocity = Vector2.up * speed;
            }
                
        }
    }

    private bool IsNearCeiling()
    {
        var hitInfo = Physics2D.Raycast(transform.position, Vector2.up, isNearCeilingRayDistance, 1 << 10);
        return hitInfo.collider != null;
    }

    private void ChangeAnimationState(string newState)
    {
        if (_currentState == newState) return;
        anim.Play(newState);
        _currentState = newState;
    }
    
}
