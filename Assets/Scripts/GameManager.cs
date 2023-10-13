using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton

    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("GameManager is NULL");
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance) Destroy(gameObject);
        else _instance = this;
        DontDestroyOnLoad(this);
    }

    #endregion

    public int MaxHealth;
    public int CurrentHealth;
    public PlayerController Player;
    [SerializeField] private GameObject goblin;
    [SerializeField] private Transform spawner;

    public void Start()
    {
        Player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        CurrentHealth = MaxHealth;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))  Instantiate(goblin, spawner);
    }

    public void LooseHealth(int dmg)
    {
        if (CurrentHealth == 0) return;
        if (CurrentHealth <= dmg)
        {
            Death();
            Player.Instance.Death();
            CurrentHealth = 0;
        }
        else
        {
            CurrentHealth -= dmg;
        }
        
    }

    public void Death()
    {
    }
}
