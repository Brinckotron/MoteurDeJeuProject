using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCrystal : Pickable
{
    public override void OnPickup(GameObject player)
    {
        int healthValue = 5*Random.Range(1, 5);
        GameManager.Instance.GainHealth(healthValue);
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
