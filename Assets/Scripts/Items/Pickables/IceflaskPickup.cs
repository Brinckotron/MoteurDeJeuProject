using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceflaskPickup : Pickable
{
    public override void OnPickup(GameObject player)
    {
        var pickUp = Instantiate(pickUpEffect, transform.position + (Vector3.up * 0.5f), transform.rotation);
        GameManager.Instance.iceFlaskAmount++;
        pickUp.gameObject.GetComponent<PickUpCounter>().sR.sprite =
            this.gameObject.GetComponent<SpriteRenderer>().sprite;
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