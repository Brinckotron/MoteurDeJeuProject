using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GoldCoin : Pickable
{

    [SerializeField] private int minGoldValue, maxGoldValue;
    private int _goldValue;

    private void Awake()
    {
        _goldValue = Random.Range(minGoldValue, maxGoldValue+1);
    }

    public override void OnPickup(GameObject player)
    {
        GameManager.Instance.GainGold(_goldValue);
        Instantiate(pickUpEffect, player.transform.position, player.transform.rotation);
        PlaySound(audioSource, audioClip);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 9)
        {
            OnPickup(other.gameObject);
        }
    }
}
