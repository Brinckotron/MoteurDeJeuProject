using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickable : MonoBehaviour
{
    private CircleCollider2D _coll;
    [SerializeField] public GameObject pickUpEffect;
    public abstract void OnPickup(GameObject player);

}
