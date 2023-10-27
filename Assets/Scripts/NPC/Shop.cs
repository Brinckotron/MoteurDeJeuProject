using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private GameObject shopWindow;
    [SerializeField] private TMP_Text shopText;
    private bool _isInRange;
    private bool _isShopOpen;

    private void Update()
    {
        if (GameManager.Instance.GameState == GameManager.Status.Play && _isInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (_isShopOpen)
            {
                _isShopOpen = false;
                shopWindow.gameObject.SetActive(false);
            }
            else
            {
                _isShopOpen = true;
                shopWindow.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 9)
        {
            _isInRange = true;
            shopText.enabled = true;    
        }
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 9)
        {
            _isInRange = false;
            shopText.enabled = false;
            if (_isShopOpen)
            {
                _isShopOpen = false;
                shopWindow.gameObject.SetActive(false);
            }
        }
    }
}
