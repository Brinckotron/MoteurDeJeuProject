using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Throwable : MonoBehaviour
{

    [SerializeField] private float gravity;
    [SerializeField] private float speed;
    [SerializeField] private int impactDamage;
    [SerializeField] private bool hasAoE;
    [SerializeField] private Rigidbody2D rb2d;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip impactSound;
    [SerializeField] private GameObject splashPrefab;
    private CircleCollider2D _coll;

    private void Start()
    {
        if (!hasAoE) splashPrefab = null;
        rb2d.gravityScale = gravity;
    }

    public void Launch(float direction)
    {
        rb2d.velocity = new Vector2(direction * speed, (rb2d.velocity.y + 0.2f) * speed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasAoE)
        {
            Instantiate(splashPrefab, transform.position, Quaternion.identity);
        }
        else if(other.gameObject.layer == 12)
            other.gameObject.GetComponentInParent<EnemyBehaviour>().TakeDamage(impactDamage);
        //PlaySound(audioSource, impactSound);
        Destroy(gameObject);
    }
    
    public virtual void PlaySound(AudioSource source, AudioClip clip, float pitch = 1f, float volume = 0.5f)
    {
        var soundPoint = Instantiate(source, transform);
        soundPoint.clip = clip;
        soundPoint.volume = Mathf.Clamp(volume, 0f, 1f) * GameManager.Instance.gameSoundVolume;
        soundPoint.pitch = Mathf.Clamp(pitch, 0f, 2f);
        soundPoint.Play();
        Destroy(soundPoint, clip.length);

    }
}
