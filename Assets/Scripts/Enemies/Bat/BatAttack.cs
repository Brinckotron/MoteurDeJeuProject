using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatAttack : MonoBehaviour
{
   private BatController _bat;
   private int _damage;

   private void Awake()
   {
      _bat = gameObject.GetComponentInParent<BatController>();
      _damage = (int)_bat.AtkDamage;
   }

   private void OnTriggerEnter2D(Collider2D other)
   {
      if (other.gameObject.layer == 9)
      {
         other.gameObject.GetComponentInParent<PlayerController>().TakeDamage(_damage, _bat.gameObject);
      }
   }
}
