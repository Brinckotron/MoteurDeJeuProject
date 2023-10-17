using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GoblinSpawner : MonoBehaviour
{
    [SerializeField] private GameObject goblin;
    [SerializeField] private Transform spawner;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) Instantiate(goblin, spawner);
    }
}