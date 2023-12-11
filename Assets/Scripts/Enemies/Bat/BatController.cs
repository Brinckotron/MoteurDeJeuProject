using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class BatController : FlyerEnemy
{
    private string _currentState;
    [SerializeField] private float isNearCeilingRayDistance = 0.11f;
    [SerializeField] private float bloodLustSpeed;
    [SerializeField] private float bloodLustAtkDelay;
    [SerializeField] private float bloodLustDuration;
    [SerializeField] private float bloodLustSightRange;
    [SerializeField] private float nagDistance;
    [SerializeField] private float atkRange;
    [SerializeField] private AudioClip deathSound;
    private Vector2 _nagPoint;
    private float _bloodLustTimer;
    private float _atkDuration = 0.25f;
    private float _atkDurationTimer;
    private bool _isBloodLust;
    private bool _isDeathSoundPlayed;
    private bool _isNagging;

    private void Awake()
    {
        HurtDuration = 0.25f;
    }

    private void Start()
    {
        Player = GameObject.FindWithTag("Player");
        PlayerCollider = Player.GetComponentInChildren<CapsuleCollider2D>();
        Rb2D = GetComponent<Rigidbody2D>();
        sR = GetComponent<SpriteRenderer>();
        MainCollider = GetComponent<CircleCollider2D>();
        CurrentHealth = maxHealth;
    }

    private void FixedUpdate()
    {
        if (_atkDurationTimer > 0) _atkDurationTimer -= Time.deltaTime;
        if (AtkDelayTimer > 0) AtkDelayTimer -= Time.deltaTime;
        Hurt();
        Death();

        var distanceFromPlayer = Vector2.Distance(Player.transform.position, transform.position);
        if (!IsHurt && !IsDead && GameManager.Instance.GameState == GameManager.Status.Play)
        {
            KeepPlayerPositionMemorized();
            if (!CanSeePlayer() && MemorizedPlayerPosition == null)
            {
                GoPerch();
            }
            else
            {
                MoveIn(distanceFromPlayer);
            }
        }
        if (GameManager.Instance.GameState == GameManager.Status.ArenaLoad) Stop();
    }

    private void MoveIn(float distanceFromPlayer)
    {
        if (distanceFromPlayer > nagDistance)
        {
            MoveTowardsPlayer();
            FacePlayer();
            ChangeAnimationState("Fly");
        }
        else
        {
            NagOrAttack(distanceFromPlayer);
        }
    }

    private void NagOrAttack(float distanceFromPlayer)
    {
        if (AtkDelayTimer > 0)
        {
            if(_atkDurationTimer > 0) Stop();
            else
            {
                Nag();
                ChangeAnimationState("Fly");
            }
        }
        else
        {
            if (distanceFromPlayer > atkRange)
            {
                FacePlayer();
                MoveTowardsPlayer();
            }
            else
            {
                FacePlayer();
                Attack();
                ChangeAnimationState("Attack");
            }
        }
    }

    private void GoPerch()
    {
        if (IsNearCeiling())
        {
            Rb2D.velocity = Vector2.zero;
            ChangeAnimationState("Perched");
        }
        else
        {
            Rb2D.velocity = Vector2.up * (speed / 2);
            ChangeAnimationState("Fly");
        }
    }

    private void Nag()
    {
        if (Vector2.Distance(_nagPoint, transform.position) < 0.2f)
        {
            _isNagging = false;
            Rb2D.velocity = Vector2.zero;
        }

        if (!_isNagging)
        {
            _nagPoint = new Vector2(Player.transform.position.x + Random.Range(-0.6f, 0.6f),
                Player.transform.position.y + Random.Range(-0f, 1.1f));
            _isNagging = true;
            Rb2D.velocity = (_nagPoint - (Vector2)transform.position).normalized * speed;
            FaceNagPoint();
        }
        else
        {
            Rb2D.velocity = (_nagPoint - (Vector2)transform.position).normalized * speed;
            FaceNagPoint();
        }
    }

    public void FaceNagPoint()
    {
        Vector3 scale = transform.localScale;
        float x = transform.position.x < _nagPoint.x ? 1f : -1f;
        transform.localScale = new Vector3(x, 1, 1);
    }

    public override void Attack()
    {
        _isNagging = false;
        AtkDelayTimer = _atkDuration + atkDelay;
        _atkDurationTimer = _atkDuration;
    }

    public override void TakeDamage(int dmg)
    {
        if (IsHurt) return;
        if (IsDead) return;
        CurrentHealth -= dmg;
        Instantiate(bloodSplatter, transform.position, transform.rotation);
        if (CurrentHealth <= 0) StartCoroutine(Die());
        else
        {
            IsHurt = true;
            ChangeAnimationState("Hurt");
            _isNagging = false;
        }
    }

    private void Hurt()
    {
        if (!IsHurt) return;
        Stop();
        if (HurtTimer > 0)
        {
            HurtTimer -= Time.deltaTime;
            if (HurtTimer <= 0)
            {
                IsHurt = false;
                AtkDelayTimer = 0.3f;
            }
        }

        if (HurtTimer <= 0 && IsHurt) HurtTimer = HurtDuration;
    }

    private void Death()
    {
        if (IsDead)
        {
           ChangeAnimationState("Die");

            if (!_isDeathSoundPlayed)
            {
                _isDeathSoundPlayed = true;
                PlaySound(audioSource, deathSound);
                Stop();
                Rb2D.gravityScale = 2;
            }
        }
    }

    public override void DeathDrop()
    {
        Instantiate(xpCrystal5, RandomDropPoint(), transform.rotation);
        if (Random.Range(1, 11) == 1) Instantiate(healthCrystal, RandomDropPoint(), transform.rotation);
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