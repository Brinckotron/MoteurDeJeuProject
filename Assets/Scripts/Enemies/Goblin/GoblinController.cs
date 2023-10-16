using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinController : LandEnemy
{
    public bool isSqueezed;
    private bool _isJumpingBack, _justGotHurt;
    [SerializeField] private float atk1Range, atk2Range;
    private readonly float _atk1Duration = 0.533f, _atk2Duration = 0.917f, _justGotHurtDuration = 1f;
    private float _atk1DurationTimer, _atk2DurationTimer, _justGotHurtTimer;
    private int _isAttacking1, _isAttacking2, _isRunning, _isDead, _isHit, _isJumping;
    private int[] idle;
    [SerializeField] private GameObject bloodSplatter;

    private void Awake()
    {
        _isAttacking1 = Animator.StringToHash("isAttacking1");
        _isAttacking2 = Animator.StringToHash("isAttacking2");
        _isRunning = Animator.StringToHash("isRunning");
        _isDead = Animator.StringToHash("isDead");
        _isHit = Animator.StringToHash("isHit");
        _isJumping = Animator.StringToHash("isJumping");
        idle = new int[] { _isAttacking1, _isAttacking2, _isRunning, _isDead, _isHit, _isJumping };
        HurtDuration = 0.25f;
    }

    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        PlayerCollider = Player.GetComponentInChildren<CapsuleCollider2D>();
        Rb2D = GetComponent<Rigidbody2D>();
        MainCollider = GetComponentInChildren<CapsuleCollider2D>();
        CurrentHealth = maxHealth;
    }

    private void Update()
    {
        if (_justGotHurtTimer > 0)
        {
            _justGotHurtTimer -= Time.deltaTime;
            if (_justGotHurtTimer <= 0) _justGotHurt = false;
        }
    }

    private void FixedUpdate()
    {
        if (!IsDead)
        {
            KeepPlayerPositionMemorized();
            if (_atk1DurationTimer <= 0 && _atk2DurationTimer <= 0 && HurtTimer <= 0) FacePlayer();
            if (!_isJumpingBack)
            {
                if (!IsInRangeForAttack1() && _atk1DurationTimer <= 0 && _atk2DurationTimer <= 0 && !IsHurt)
                {
                    MoveTowardsPlayer();
                    if (Rb2D.velocity.x != 0) SetAnimBool(_isRunning);
                    if (!CanSeePlayer() && MemorizedPlayerPosition == null) SetAnimIdle();
                }
                else if (_atk1DurationTimer <= 0 && _atk2DurationTimer <= 0)
                {
                    Stop();
                    SetAnimIdle();
                }
            }
            
            Attack();
            
            Hurt();
        }

        Death();
    }

    private void Hurt()
    {
        if (!IsHurt) return;
        if (HurtTimer > 0)
        {
            SetAnimBool(_isHit);
            HurtTimer -= Time.deltaTime;
            if (HurtTimer <= 0)
            {
                IsHurt = false;
                SetAnimIdle();
                _justGotHurt = true;
                _justGotHurtTimer = _justGotHurtDuration;
            }
        }

        if (HurtTimer <= 0 && IsHurt) HurtTimer = HurtDuration;
    }

    private void Death()
    {
        if (IsDead)
        {
            SetAnimBool(_isDead);
        }
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
        if (_atk1DurationTimer > 0)
        {
            _atk1DurationTimer -= Time.deltaTime;
            if (_atk1DurationTimer <= 0) anim.SetBool(_isAttacking1, false);
        }

        if (_atk2DurationTimer > 0)
        {
            _atk2DurationTimer -= Time.deltaTime;
            if (_atk2DurationTimer <= 0) anim.SetBool(_isAttacking2, false);
        }

        if (AtkDelayTimer > 0) AtkDelayTimer -= Time.deltaTime;

        if (Player.GetComponent<PlayerController>().Instance.CurrentPlayerState != PlayerController.PlayerState.Dead &&
            !IsHurt)
        {
            if ((IsInRangeForAttack2() || _justGotHurt) && AtkDelayTimer <= 0 && !isSqueezed)
            {
                if (_justGotHurt)AtkDelayTimer = (_atk2Duration);
                else AtkDelayTimer = (_atk2Duration + (atkDelay / 2));
                _justGotHurt = false;
                _atk2DurationTimer = _atk2Duration;
                SetAnimBool(_isAttacking2);
                StartCoroutine(Jumpback());
            }

            if (((IsInRangeForAttack1() && !IsInRangeForAttack2()) || (IsInRangeForAttack1() && isSqueezed)) && AtkDelayTimer <= 0)
            {
                _justGotHurt = false;
                AtkDelayTimer = (_atk1Duration + atkDelay);
                _atk1DurationTimer = _atk1Duration;
                SetAnimBool(_isAttacking1);
            }
        }
    }

    private IEnumerator Jumpback()
    {
        _isJumpingBack = true;
        yield return new WaitForSeconds(0.1f);
        Rb2D.velocity = (Player.transform.position.x > transform.position.x ? Vector2.left : Vector2.right) * 0.4f;
        yield return new WaitForSeconds(0.5f);
        Stop();
        _isJumpingBack = false;
    }

    public override void TakeDamage(int dmg)
    {
        if (_isJumpingBack) return;
        if (IsHurt) return;
        CurrentHealth -= dmg;
        Instantiate(bloodSplatter, transform.position, transform.rotation);
        if (CurrentHealth <= 0) StartCoroutine(Die());
        else
        {
            IsHurt = true;
            AtkDelayTimer = 0;
        }
    }

    private void SetAnimBool(int boolToSet)
    {
        foreach (var i in idle)
        {
            if (i != boolToSet) anim.SetBool(i, false);
        }

        anim.SetBool(boolToSet, true);
    }

    private void SetAnimIdle()
    {
        foreach (var i in idle)
        {
            anim.SetBool(i, false);
        }
    }
}