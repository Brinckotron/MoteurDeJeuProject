using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinController : LandEnemy
{
    public bool isSqueezed;
    private bool _isJumpingBack;
    [SerializeField] private float atk1Range, atk2Range;
    private readonly float _atk1Duration = 0.533f, _atk2Duration = 0.917f;
    private float _atk1DurationTimer, _atk2DurationTimer;
    private int _isAttacking1, _isAttacking2, _isRunning, _isDead, _isHit, _isJumping;

    private void Awake()
    {
        _isAttacking1 = Animator.StringToHash("isAttacking1");
        _isAttacking2 = Animator.StringToHash("isAttacking2");
        _isRunning = Animator.StringToHash("isRunning");
        _isDead = Animator.StringToHash("isDead");
        _isHit = Animator.StringToHash("isHit");
        _isJumping = Animator.StringToHash("isJumping");
    }

    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        PlayerCollider = Player.GetComponentInChildren<CapsuleCollider2D>();
        Rb2D = GetComponent<Rigidbody2D>();
        MainCollider = GetComponentInChildren<CapsuleCollider2D>();
    }


    void FixedUpdate()
    {
        if (_atk1DurationTimer <= 0 && _atk2DurationTimer <= 0) FacePlayer();
        if (!_isJumpingBack)
        {
            if (!IsInRangeForAttack1() && _atk1DurationTimer <= 0 && _atk2DurationTimer <= 0) MoveTowardsPlayer();
            else Stop();
        }

        if (Player.GetComponent<PlayerController>().Instance.CurrentPlayerState != "Dead") Attack();
    }

    private bool IsInRangeForAttack1()
    {
        Vector2 direction = (Player.transform.position - transform.position).normalized;
        var hitInfo = Physics2D.Raycast(transform.position, direction, atk1Range, SightLayerMask);
        return hitInfo.collider == PlayerCollider;
    }

    private bool IsInRangeForAttack2()
    {
        Vector2 direction = (Player.transform.position - transform.position).normalized;
        var hitInfo = Physics2D.Raycast(transform.position, direction, atk2Range, SightLayerMask);
        return hitInfo.collider == PlayerCollider;
    }

    public override void Attack()
    {
        if (_atk1DurationTimer <= 0) anim.SetBool(_isAttacking1, false);
        else _atk1DurationTimer -= Time.deltaTime;
        if (_atk2DurationTimer <= 0) anim.SetBool(_isAttacking2, false);
        else _atk2DurationTimer -= Time.deltaTime;
        if (AtkDelayTimer > 0) AtkDelayTimer -= Time.deltaTime;
        if (IsInRangeForAttack1() && !IsInRangeForAttack2() && AtkDelayTimer <= 0)
        {
            AtkDelayTimer = (_atk1Duration + atkDelay);
            _atk1DurationTimer = _atk1Duration;
            anim.SetBool(_isAttacking1, true);
        }

        if (IsInRangeForAttack2() && AtkDelayTimer <= 0 && !isSqueezed)
        {
            AtkDelayTimer = (_atk2Duration + atkDelay);
            _atk2DurationTimer = _atk2Duration;
            anim.SetBool(_isAttacking2, true);
            StartCoroutine(Jumpback());
        }
    }

    private IEnumerator Jumpback()
    {
        _isJumpingBack = true;
        yield return new WaitForSeconds(0.1f);
        Rb2D.velocity = (Player.transform.position.x > transform.position.x ? Vector2.left : Vector2.right) * 0.7f;
        yield return new WaitForSeconds(0.5f);
        Stop();
        _isJumpingBack = false;
    }

    public override void TakeDamage(int dmg)
    {
        if (_isJumpingBack) return;
        CurrentHealth -= dmg;
        if (CurrentHealth <= 0) Die();
    }
}