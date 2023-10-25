using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyerEnemy : EnemyBehaviour
{
    public override void MoveTowardsPlayer()
    {
        if (CanSeePlayer())
        {
            var direction = (Vector2)(Player.transform.position - transform.position).normalized;
            Rb2D.velocity = direction * speed;
        }
        else if (!CanSeePlayer() && MemorizedPlayerPosition != null)
        {
            var direction = (((Vector2)MemorizedPlayerPosition) - (Vector2)transform.position).normalized;
            Rb2D.velocity = direction * speed;
        }
    }

    public override void Stop()
    {
        Rb2D.velocity = Vector2.zero;
    }
}
