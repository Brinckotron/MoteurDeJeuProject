using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceFlaskSplash : Splash
{
    public override void DamageEffect(EnemyBehaviour target)
    {
        target.TakeDamage(damage);
        target.GetFrosted(3f);
    }
}
