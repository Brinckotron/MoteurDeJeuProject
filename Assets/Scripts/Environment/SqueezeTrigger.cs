using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SqueezeTrigger : MonoBehaviour
{
    public SqueezeTrigger instance;
    private GameObject _player;
    private PlayerController _playerScript;
    private Collider2D _playerCollider;
    private void Awake()
    {
        instance = this;
        _player = GameObject.FindWithTag("Player");
        _playerCollider = _player.GetComponentInChildren<CapsuleCollider2D>();
        _playerScript = _player.GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other == _playerCollider)
        {
            _playerScript.crouchOverride = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == _playerCollider)
        {
            _playerScript.crouchOverride = false;
        }
    }
}
