using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinAnimEvents : MonoBehaviour
{

    
    [SerializeField] private AudioSource audioSource;
    public void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
}
