using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float maxSpeed = 3.4f, jumpHeight = 6.5f, gravityScale = 1.5f;
    public Camera mainCamera;

    private bool _facingRight = true, _resetJumpNeeded;
    private float _moveDirection = 0;
    private string _anim = " ", _knightSkin = "0";
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
        Crouch,
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

        Facing();

        Jumping();

        CameraFollow();

        AnimationControl();
        
        StateControl();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void ApplyMovement()
    {
        _rb2D.velocity = new Vector2((_moveDirection) * maxSpeed, _rb2D.velocity.y);
    }

    private void Moving()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            _moveDirection = Input.GetAxisRaw("Horizontal");
        }
        else _moveDirection = 0;
    }

    private void Jumping()
    {
        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            _rb2D.velocity = new Vector2(_rb2D.velocity.x, jumpHeight);
            ResetJump();
        }
    }

    private void Facing()
    {
        if (_moveDirection != 0)
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
        var hitInfo = Physics2D.Raycast(transform.position, Vector2.down, 0.4f, 1 << 7);
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
        // Camera follow
        if (mainCamera)
        {
            mainCamera.transform.position = new Vector3(_t.position.x, _t.position.y, _cameraPos.z);
        }
    }

    private void StateControl()
    {
        if (IsGrounded())
        {
            if (_moveDirection == 0 && !_resetJumpNeeded) ChangePlayerState(PlayerState.Idle);
            if (_moveDirection != 0  && !_resetJumpNeeded) ChangePlayerState(PlayerState.Run);
            if (Input.GetKey(KeyCode.S) && !_resetJumpNeeded) ChangePlayerState(PlayerState.Run);
        }
        else
        {
            ChangePlayerState(PlayerState.Jump);
        }
        

    }

    private void ChangePlayerState(PlayerState newState)
    {
        if (_currentPlayerState != newState)
        {
            _currentPlayerState = newState;
        }
    }
    private void AnimationControl()
    {
        _anim = _currentPlayerState switch
        {
            PlayerState.Idle => "Idle",
            PlayerState.Run => "Run",
            PlayerState.Jump => "Jump",
            PlayerState.Attack => "Attack",
            PlayerState.Attack2 => "Attack_2",
            PlayerState.Block => "Block",
            PlayerState.JumpAttack => "Jump_Attack",
            PlayerState.Crouch => "Crouch",
            PlayerState.CrouchAttack => "Crouched_Attack",
            PlayerState.CrouchBlock => "Crouched_Block",
            PlayerState.Roll => "Roll",
            PlayerState.FaceWall => "Face_Wall",
            PlayerState.Ladder => "Ladder",
            PlayerState.Climb => "Climb",
            PlayerState.Hurt => "Hurt",
            PlayerState.Dead => "Dead",
            _ => "Idle"
        };
        var animState = $"Knight_{_knightSkin}_{_anim}";
        PlayerAnimationControl.instance.ChangeAnimationState(animState);
    }
}