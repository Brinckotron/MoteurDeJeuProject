using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandEnemy : EnemyBehaviour
{
    private float _patrolTimer;
    [SerializeField] private float isGroundedRayDistance;
    private bool _isPatrolMoving;
    private int _patrolDirection = 0;
    [SerializeField] private float jumpHeight = 3.5f;

    public override void MoveTowardsPlayer()
    {
        if (CanSeePlayer())
        {
            Vector2 direction = (Player.transform.position - transform.position).normalized;
            Rb2D.linearVelocity = new Vector2(direction.x * speed, Rb2D.linearVelocity.y);
            if (IsGrounded() && IsBumpingWall()) HighJump();
            if (IsGrounded() && IsNearLedge() && (Player.transform.position.y + 0.1f) >= transform.position.y) LongJump();
        }
        else if (MemorizedPlayerPosition != null)
        {
            Vector2 direction = ((Vector2)MemorizedPlayerPosition - (Vector2)transform.position).normalized;
            Rb2D.linearVelocity = new Vector2(direction.x * speed, Rb2D.linearVelocity.y);
            if (IsGrounded() && IsBumpingWall()) HighJump();
            if (IsGrounded() && IsNearLedge() && (((Vector2)MemorizedPlayerPosition).y + 0.1f) >= transform.position.y) LongJump();
        }
        else Patrol();
    }

    public override void Stop()
    {
        if (Rb2D) Rb2D.linearVelocity = new Vector2(0, Rb2D.linearVelocity.y);
    }

    public virtual bool IsGrounded()
    {
        var hitInfo = Physics2D.Raycast(transform.position, Vector2.down, isGroundedRayDistance, 1 << 10);
        return hitInfo.collider != null;
    }

    public virtual bool IsNearLedge()
    {
        var hitInfo = Physics2D.Raycast(castPos.position, Vector2.down, 0.5f, 1 << 10);
        return hitInfo.collider == null;
    }
    
    public virtual bool IsNearLedgePatrol()
    {
        var hitInfo = Physics2D.Raycast(castPos.position, Vector2.down, 0.05f, 1 << 10);
        return hitInfo.collider == null;
    }

    public virtual bool IsBumpingWall()
    {
        var hitInfo = Physics2D.Raycast(castPos.position, transform.localScale.x == 1 ? Vector2.right : Vector2.left,
            0.05f, 1 << 10);
        return hitInfo.collider != null;
    }

    public virtual void HighJump()
    {
        Rb2D.linearVelocity = new Vector2(Rb2D.linearVelocity.x, jumpHeight);
    }

    public virtual void LongJump()
    {
        Rb2D.linearVelocity = new Vector2(Rb2D.linearVelocity.x+1, jumpHeight);
    }

    public virtual void Patrol()
    {
        if (_patrolTimer > 0)
        {
            _patrolTimer -= Time.deltaTime;
            if (_isPatrolMoving && IsGrounded() && (IsNearLedgePatrol() || IsBumpingWall()))
            {
                if (Random.Range(0, 2) == 1) _isPatrolMoving = false;
                else
                {
                    _patrolDirection = -_patrolDirection;
                    transform.localScale = new Vector3(_patrolDirection, 1, 1);
                }
            }
        }

        if (_patrolTimer <= 0)
        {
            _isPatrolMoving = Random.Range(0, 3) >= 1;
            if (_isPatrolMoving)
            {
                var rndDir = Random.Range(0, 2);
                _patrolDirection = rndDir == 0 ? 1 : -1;
                transform.localScale = new Vector3(_patrolDirection, 1, 1);
                if (IsGrounded() && (IsNearLedge() || IsBumpingWall()))
                {
                    _patrolDirection = -_patrolDirection;
                    transform.localScale = new Vector3(_patrolDirection, 1, 1);
                }
            }

            _patrolTimer = Random.Range(2, 6);
        }

        if (_isPatrolMoving) Rb2D.linearVelocity = new Vector2(_patrolDirection * (speed / 2), Rb2D.linearVelocity.y);
        else Rb2D.linearVelocity = new Vector2(0, Rb2D.linearVelocity.y);
    }
}