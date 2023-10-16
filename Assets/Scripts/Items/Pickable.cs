using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickable : MonoBehaviour
{
    private CircleCollider2D _coll;
    private int _value;

    public int Value
    {
        get { return _value; }
        set { _value = value; }
    }
    public abstract void OnPickup(PlayerController player);

}
