using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Throwable : MonoBehaviour
{

    [SerializeField]private float gravity;
    [SerializeField] private float speed;
    [SerializeField]private int impactDamage;
    [SerializeField]private bool hasAoE;
    [SerializeField]private Rigidbody2D rb2d;
    private GameObject _splashPrefab;
    private CircleCollider2D _coll;

    private void Start()
    {
        if (!hasAoE) _splashPrefab = null;
        rb2d.gravityScale = gravity;
    }

    public void Launch(float direction)
    {
        rb2d.velocity = new Vector2(direction * speed, (rb2d.velocity.y + 0.2f) * speed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 12)
            other.gameObject.GetComponentInParent<EnemyBehaviour>().TakeDamage(impactDamage);
        if (hasAoE)
        {
            Instantiate(_splashPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
