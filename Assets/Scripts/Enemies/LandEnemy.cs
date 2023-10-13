using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandEnemy : EnemyBehaviour
{

    public override void MoveTowardsPlayer()
    {
        if (CanSeePlayer())
        {
            Vector2 direction = (Player.transform.position - transform.position).normalized;
            Rb2D.velocity = new Vector2(direction.x * speed, Rb2D.velocity.y);
        }
        else if (MemorizedPlayerPosition != null) 
        {
            Vector2 direction = ((Vector2)MemorizedPlayerPosition - (Vector2)transform.position).normalized;
            Rb2D.velocity = new Vector2(direction.x * speed, Rb2D.velocity.y);
        }
        else Stop();
    }
    
    public override void Stop()
    {
        Rb2D.velocity = new Vector2(0, Rb2D.velocity.y);
    }

}
