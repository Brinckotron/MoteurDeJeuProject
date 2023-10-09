using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float maxSpeed, jumpHeight, gravityScale, rollSpeed, rollStaminaCost, blockStaminaCost;
    public Camera mainCamera;
    public bool crouchOverride = false;
    
    private bool _facingRight = true,
        _resetJumpNeeded,
        _isJumpPressed = false,
        _isRollPressed = false,
        _isAttackPressed = false,
        _isBlockPressed = false,
        _isSecondaryAttackPressed = false,
        _isDead = false;

    private float _moveDirection = 0,
        _attackLength = 0.3f,
        _attackDelayTimer,
        _secondaryAttackDelayTimer,
        _secondaryAttackLength = 0.44f,
        _rollLength = 0.35f,
        _rollDelayTimer,
        _rollCancelDelay = 0.2f,
        _rollCancelTimer;

    private string _knightSkin = "0", _debugState = " ", _newDebugState = " ";
    private Vector3 _cameraPos;
    private Rigidbody2D _rb2D;
    private CapsuleCollider2D _collider;
    private Transform _t;

    private enum PlayerState
    {
        Idle,
        Run,
        Attack,
        Attack2,
        Block,
        Jump,
        JumpAttack,
        Falling,
        Crouch,
        Crouched,
        CrouchAttack,
        CrouchBlock,
        Roll,
        FaceWall,
        Ladder,
        Climb,
        Hurt,
        Dead
    }

    private PlayerState _currentPlayerState = PlayerState.Idle;


    private void Start()
    {
        _t = transform;
        _rb2D = GetComponent<Rigidbody2D>();
        _collider = GetComponentInChildren<CapsuleCollider2D>();
        _rb2D.gravityScale = gravityScale;
        _facingRight = _t.localScale.x > 0;

        if (mainCamera)
        {
            _cameraPos = mainCamera.transform.position;
        }
    }

    private void Update()
    {
        Moving();

        Attacking();

        Attack2();

        Facing();

        Blocking();

        Jumping();

        Rolling();
        
        CancelRoll();
        
        Death();

        CameraFollow();
    }

    private void FixedUpdate()
    {
        ApplyMovement();

        AnimationControl();

        StateControl();
    }

    private void ApplyMovement()
    {
        if (_currentPlayerState is PlayerState.Idle or PlayerState.Run or PlayerState.Jump or PlayerState.Falling)
            _rb2D.velocity = new Vector2((_moveDirection) * maxSpeed, _rb2D.velocity.y);
        else if (_currentPlayerState is PlayerState.Attack or PlayerState.CrouchAttack or PlayerState.Attack2
                 or PlayerState.Dead or PlayerState.Block or PlayerState.CrouchBlock
                 or PlayerState.Crouch or PlayerState.Crouched) _rb2D.velocity = Vector2.zero;
        else if (_currentPlayerState == PlayerState.Roll)
        {
            if (_facingRight) _rb2D.velocity = new Vector2(rollSpeed, _rb2D.velocity.y);
            else _rb2D.velocity = new Vector2(-rollSpeed, _rb2D.velocity.y);
        }
    }

    private void Moving()
    {
        if (_currentPlayerState != PlayerState.Dead && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
        {
            _moveDirection = Input.GetAxisRaw("Horizontal");
        }
        else _moveDirection = 0;
    }

    private void Jumping()
    {
        if (_isJumpPressed && !IsGrounded()) _isJumpPressed = false;
        if (Input.GetKey(KeyCode.W) && IsGrounded() &&
            _currentPlayerState is PlayerState.Idle or PlayerState.Run)
        {
            _rb2D.velocity = new Vector2(_rb2D.velocity.x, jumpHeight);
            _isJumpPressed = true;
        }
    }

    private void Rolling()
    {
        if (_rollDelayTimer > 0)
        {
            _rollDelayTimer -= Time.deltaTime;
            _isRollPressed = true;
        }

        if (Input.GetKeyDown(KeyCode.Space) && _rollDelayTimer <= 0f && IsGrounded() &&
            _currentPlayerState is not (PlayerState.Attack or PlayerState.Attack2 or PlayerState.CrouchAttack))
        {
            _rollDelayTimer = _rollLength;
        }
    }

    private void CancelRoll()
    {
        if (_rollCancelTimer > 0) _rollCancelTimer -= Time.deltaTime;
        if (Input.GetKeyUp(KeyCode.S) && _rollCancelTimer > 0)
        {
            _rollDelayTimer = 0f;
            _rollCancelTimer = 0f;
        }
        if (Input.GetKeyDown(KeyCode.S) && _rollCancelTimer <= 0) _rollCancelTimer = _rollCancelDelay;

    }

    private void Attacking()
    {
        if (_attackDelayTimer > 0)
        {
            _attackDelayTimer -= Time.deltaTime;
            _isAttackPressed = true;
        }

        if (!Input.GetKey(KeyCode.LeftShift) && Input.GetButtonDown("Fire1") && _attackDelayTimer <= 0f && _rollDelayTimer <= 0.1f)
        {
            _attackDelayTimer = _attackLength;
        }
    }

    private void Attack2()
    {
        if (_secondaryAttackDelayTimer > 0)
        {
            _secondaryAttackDelayTimer -= Time.deltaTime;
            _isSecondaryAttackPressed = true;
        }

        if (IsGrounded() && Input.GetKey(KeyCode.LeftShift) && Input.GetButtonDown("Fire1") &&
            _secondaryAttackDelayTimer <= 0f && !crouchOverride)
        {
            _secondaryAttackDelayTimer = _secondaryAttackLength;
        }
    }

    private void Blocking()
    {
        if (Input.GetButtonDown("Fire2")) _isBlockPressed = true;
        else if (Input.GetButtonUp("Fire2")) _isBlockPressed = false;
    }

    private void Facing()
    {
        if (_moveDirection != 0 && _currentPlayerState is not (PlayerState.Attack or PlayerState.Attack2
                or PlayerState.CrouchAttack or PlayerState.JumpAttack or PlayerState.Roll))
        {
            if (_moveDirection > 0 && !_facingRight)
            {
                _facingRight = true;
                _t.localScale = new Vector3(Mathf.Abs(_t.localScale.x), _t.localScale.y, _t.localScale.z);
            }

            if (_moveDirection < 0 && _facingRight)
            {
                _facingRight = false;
                _t.localScale = new Vector3(-Mathf.Abs(_t.localScale.x), _t.localScale.y, _t.localScale.z);
            }
        }
    }

    private bool IsGrounded()
    {
        var hitInfo = Physics2D.Raycast(transform.position, Vector2.down, 0.35f, 1 << 7);
        return hitInfo.collider != null && !_resetJumpNeeded;
    }

    private IEnumerator ResetJump()
    {
        _resetJumpNeeded = true;
        yield return new WaitForSeconds(0.1f);
        _resetJumpNeeded = false;
    }

    private void CameraFollow()
    {
        if (mainCamera)
        {
            mainCamera.transform.position = new Vector3(_t.position.x, _t.position.y, _cameraPos.z);
        }
    }

    private void StateControl()
    {
        if (_isDead)
        {
            ChangePlayerState(PlayerState.Dead);
            return;
        }
        
        if (IsGrounded())
        {
            if (crouchOverride && _attackDelayTimer <= 0 && _rollDelayTimer <= 0 &&
                _secondaryAttackDelayTimer <= 0)
            {
                ChangePlayerState(_currentPlayerState is (PlayerState.Crouched or PlayerState.CrouchAttack
                    or PlayerState.CrouchBlock or PlayerState.Roll)
                    ? PlayerState.Crouched
                    : PlayerState.Crouch);
            }
            if (_moveDirection == 0 && !_resetJumpNeeded && _attackDelayTimer <= 0 && _rollDelayTimer <= 0 &&
                _secondaryAttackDelayTimer <= 0)
            {
                if (Input.GetKey(KeyCode.S) || crouchOverride)
                {
                    ChangePlayerState(_currentPlayerState is (PlayerState.Crouched or PlayerState.CrouchAttack
                        or PlayerState.CrouchBlock or PlayerState.Roll)
                        ? PlayerState.Crouched
                        : PlayerState.Crouch);
                }
                else ChangePlayerState(PlayerState.Idle);
            }


            if (_isBlockPressed && _attackDelayTimer <= 0 && _rollDelayTimer <= 0 && _secondaryAttackDelayTimer <= 0 &&
                _currentPlayerState is PlayerState.Crouch or PlayerState.Crouched or PlayerState.Idle or PlayerState.Run
                    or PlayerState.Attack or PlayerState.Attack2 or PlayerState.Roll or PlayerState.CrouchAttack
                    or PlayerState.Falling)
            {
                ChangePlayerState(
                    _currentPlayerState is PlayerState.Crouch or PlayerState.Crouched or PlayerState.CrouchAttack
                        or PlayerState.CrouchBlock
                        ? PlayerState.CrouchBlock
                        : PlayerState.Block);
            }

            if (_moveDirection != 0 && !_isBlockPressed && !_isAttackPressed &&
                !_isSecondaryAttackPressed && !crouchOverride) ChangePlayerState(PlayerState.Run);
            if (_isAttackPressed)
            {
                if (_attackDelayTimer <= 0) _isAttackPressed = false;
                if (_currentPlayerState == PlayerState.JumpAttack && _attackDelayTimer < (_attackLength - 0.05f))
                    _attackDelayTimer = 0;

                else
                {
                    ChangePlayerState(
                        _currentPlayerState is PlayerState.Crouch or PlayerState.Crouched or PlayerState.CrouchAttack
                            or PlayerState.CrouchBlock or PlayerState.Roll
                            ? PlayerState.CrouchAttack
                            : PlayerState.Attack);
                }
            }

            if (_isSecondaryAttackPressed)
            {
                if (_secondaryAttackDelayTimer <= 0) _isSecondaryAttackPressed = false;
                ChangePlayerState(PlayerState.Attack2);
            }

            if (_isRollPressed)
            {
                if (_rollDelayTimer <= 0) _isRollPressed = false;
                ChangePlayerState(PlayerState.Roll);
            }

            if (_isJumpPressed)
            {
                ChangePlayerState(PlayerState.Jump);
                StartCoroutine(ResetJump());
            }
        }
        else
        {
            if (_isAttackPressed)
            {
                if (_attackDelayTimer <= 0) _isAttackPressed = false;
                ChangePlayerState(PlayerState.JumpAttack);
            }

            if (_currentPlayerState == PlayerState.JumpAttack && _attackDelayTimer <= 0)
                ChangePlayerState(PlayerState.Falling);
            if (_rb2D.velocity.y < 0 && _currentPlayerState != PlayerState.JumpAttack && _rollDelayTimer <= 0)
                ChangePlayerState(PlayerState.Falling);
        }
    }

    private void ChangePlayerState(PlayerState newState)
    {
        if (_currentPlayerState != newState)
        {
            _currentPlayerState = newState;
            /*debug*/
            _newDebugState = _currentPlayerState.ToString();
        }

        //debug
        if (_debugState != _newDebugState)
        {
            _debugState = _newDebugState;
            if (Input.GetKey(KeyCode.LeftControl)) Debug.Log(_debugState);
        }
    }

    private void AnimationControl()
    {
        var anim = " ";
        anim = _currentPlayerState switch
        {
            PlayerState.Idle => "Idle",
            PlayerState.Run => "Run",
            PlayerState.Jump => "Jump",
            PlayerState.Attack => "Attack",
            PlayerState.Attack2 => "Attack_2",
            PlayerState.Block => "Block",
            PlayerState.JumpAttack => "Jump_Attack",
            PlayerState.Falling => "Falling",
            PlayerState.Crouch => "Crouch",
            PlayerState.Crouched => "Crouched",
            PlayerState.CrouchAttack => "Crouched_Attack",
            PlayerState.CrouchBlock => "Crouched_Block",
            PlayerState.Roll => "Roll",
            PlayerState.FaceWall => "Face_Wall",
            PlayerState.Ladder => "Ladder",
            PlayerState.Climb => "Climb",
            PlayerState.Hurt => "Hurt",
            PlayerState.Dead => "Death",
            _ => "Idle"
        };
        var animState = $"Knight_{_knightSkin}_{anim}";
        PlayerAnimationControl.instance.ChangeAnimationState(animState);
    }

    private void Death()
    {
        if (Input.GetKeyDown(KeyCode.K)) _isDead = true;
    }
}