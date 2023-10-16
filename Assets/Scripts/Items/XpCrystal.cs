using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XpCrystal : Pickable
{

    [SerializeField] private int xpValue;
    public override void OnPickup(GameObject player)
    {
        GameManager.Instance.GainXp(xpValue);
        Instantiate(pickUpEffect, player.transform.position, player.transform.rotation);
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
