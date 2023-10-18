using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickable : MonoBehaviour
{
    private CircleCollider2D _coll;
    [SerializeField] public GameObject pickUpEffect;
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioClip audioClip;
    public abstract void OnPickup(GameObject player);

    public virtual void PlaySound(AudioSource source, AudioClip clip, float pitch = 1f)
    {
        var soundPoint = Instantiate(source, transform.position, transform.rotation);
        soundPoint.clip = clip;
        soundPoint.volume = 0.1f;
        soundPoint.pitch = Mathf.Clamp(pitch, 0f, 2f);
        soundPoint.Play();
        Destroy(soundPoint, clip.length);

    }

}
