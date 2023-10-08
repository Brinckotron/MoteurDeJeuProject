using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SqueezeTrigger : MonoBehaviour
{
    public SqueezeTrigger instance;
    private GameObject _player;
    private PlayerController _playerScript;
    private void Awake()
    {
        instance = this;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("SqueezeTrigger Entered");
        
        if (other.gameObject.CompareTag("Player"))
        {
            _playerScript = other.GetComponentInParent<PlayerController>();
            _playerScript.crouchOverride = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _playerScript = other.GetComponentInParent<PlayerController>();
            _playerScript.crouchOverride = false;
        }
    }
}
