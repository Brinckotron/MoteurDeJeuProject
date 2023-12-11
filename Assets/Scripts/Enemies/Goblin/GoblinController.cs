using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GoblinController : LandEnemy
{
    public bool isSqueezed;
    private bool _isJumpingBack, _isForwardRolling, _justGotHurt, _isDeathSoundPlayed = false;
    [SerializeField] private float atk1Range, atk2Range;
    private readonly float _atk1Duration = 0.533f, _atk2Duration = 0.917f, _justGotHurtDuration = 1f;
    private float _atk1DurationTimer, _atk2DurationTimer, _justGotHurtTimer, _voicePitch;
    private int _isAttacking1, _isAttacking2, _isRunning, _isDead, _isHit, _isJumping;
    private int[] _idle;
    [SerializeField] private AudioClip laugh;
    [SerializeField] private AudioClip[] deathClips;

    private void Awake()
    {
        _isAttacking1 = Animator.StringToHash("isAttacking1");
        _isAttacking2 = Animator.StringToHash("isAttacking2");
        _isRunning = Animator.StringToHash("isRunning");
        _isDead = Animator.StringToHash("isDead");
        _isHit = Animator.StringToHash("isHit");
        _isJumping = Animator.StringToHash("isJumping");
        _idle = new int[] { _isAttacking1, _isAttacking2, _isRunning, _isDead, _isHit, _isJumping };
        HurtDuration = 0.25f;
        _voicePitch = Random.Range(1f, 1.2f);
    }

    private void Start()
    {
        Player = GameObject.FindWithTag("Player");
        PlayerCollider = Player.GetComponentInChildren<CapsuleCollider2D>();
        Rb2D = GetComponent<Rigidbody2D>();
        sR = GetComponentInChildren<SpriteRenderer>();
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
        if (!IsDead && GameManager.Instance.GameState == GameManager.Status.Play)
        {
            KeepPlayerPositionMemorized();
            if (_atk1DurationTimer <= 0 && _atk2DurationTimer <= 0 && HurtTimer <= 0 && !_isForwardRolling) FacePlayer();
            if (!_isJumpingBack && !_isForwardRolling)
            {
                if (!IsInRangeForAttack1() && _atk1DurationTimer <= 0 && _atk2DurationTimer <= 0 && !IsHurt)
                {
                    MoveTowardsPlayer();
                    if (Rb2D.velocity.x != 0) SetAnimBool(_isRunning);
                    if (!CanSeePlayer() && MemorizedPlayerPosition == null && Rb2D.velocity.x == 0) SetAnimIdle();
                    if (!IsGrounded() && Rb2D.velocity.y > 0) SetAnimBool(_isJumping);
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

        if (GameManager.Instance.GameState == GameManager.Status.ArenaLoad)
        {
            Stop();
            SetAnimIdle();
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
            if (!_isDeathSoundPlayed)
            {
                _isDeathSoundPlayed = true;
                PlaySound(audioSource, deathClips[Random.Range(0, 4)], _voicePitch);
            }
        }
    }

    public override void DeathDrop()
    {
        switch (Random.Range(0, 2))
        {
            case 0:
                Instantiate(xpCrystal10, RandomDropPoint(), transform.rotation);
                break;
            case 1:
                Instantiate(xpCrystal5, RandomDropPoint(), transform.rotation);
                Instantiate(xpCrystal5, RandomDropPoint(), transform.rotation);
                break;
        }
        if (Random.Range(1, 6) == 1) Instantiate(healthCrystal,RandomDropPoint(), transform.rotation);
        switch (Random.Range(0, 6))
        {
            case <= 3:
                break;
            case 4:
                Instantiate(goldCoin, RandomDropPoint(), transform.rotation);
                break;
            case 5:
                Instantiate(goldCoins, RandomDropPoint(), transform.rotation);
                break;
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
            !IsHurt && IsGrounded())
        {
            if ((IsInRangeForAttack2() || _justGotHurt) && AtkDelayTimer <= 0 && !isSqueezed)
            {
                if (Random.Range(1, 5) == 1)
                {
                    
                    AtkDelayTimer = atkDelay;
                    _justGotHurt = false;
                    StartCoroutine(ForwardRoll());
                }
                else
                {
                    if (_justGotHurt) AtkDelayTimer = (_atk2Duration);
                    else AtkDelayTimer = (_atk2Duration + (atkDelay / 2));
                    _justGotHurt = false;
                    _atk2DurationTimer = _atk2Duration;
                    SetAnimBool(_isAttacking2);
                    StartCoroutine(Jumpback());
                }
            }

            if (((IsInRangeForAttack1() && !IsInRangeForAttack2()) || (IsInRangeForAttack1() && isSqueezed)) &&
                AtkDelayTimer <= 0)
            {
                if (Random.Range(1, 7) == 1)
                {
                    AtkDelayTimer = atkDelay;
                    _justGotHurt = false;
                    StartCoroutine(ForwardRoll());
                }
                else
                {
                    _justGotHurt = false;
                    AtkDelayTimer = (_atk1Duration + atkDelay);
                    _atk1DurationTimer = _atk1Duration;
                    SetAnimBool(_isAttacking1);
                    
                }
            }
        }
    }

    private IEnumerator ForwardRoll()
    {
        _isForwardRolling = true;
        SetAnimBool(_isJumping);
        if (Random.Range(1, 4) == 1) PlaySound(audioSource, laugh, _voicePitch);
        Rb2D.velocity = (Player.transform.position.x > transform.position.x ? Vector2.right : Vector2.left) * 0.8f;
        yield return new WaitForSeconds(0.8f);
        Stop();
        _isForwardRolling = false;
    }

    private IEnumerator Jumpback()
    {
        _isJumpingBack = true;
        if (Random.Range(1, 4) == 1) PlaySound(audioSource, laugh, _voicePitch);
        yield return new WaitForSeconds(0.1f);
        Rb2D.velocity = (Player.transform.position.x > transform.position.x ? Vector2.left : Vector2.right) * 0.4f;
        yield return new WaitForSeconds(0.5f);
        Stop();
        _isJumpingBack = false;
    }

    public override void TakeDamage(int dmg)
    {
        if (_isJumpingBack) return;
        if (_isForwardRolling) return;
        if (IsHurt) return;
        if (IsDead) return;
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
        foreach (var i in _idle)
        {
            if (i != boolToSet) anim.SetBool(i, false);
        }

        anim.SetBool(boolToSet, true);
    }

    private void SetAnimIdle()
    {
        foreach (var i in _idle)
        {
            anim.SetBool(i, false);
        }
    }
}