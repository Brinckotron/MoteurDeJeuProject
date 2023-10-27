using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinAnimEvents : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    public void PlaySound(AudioClip clip)
    {
        audioSource.volume = GameManager.Instance.gameSoundVolume;
        audioSource.clip = clip;
        audioSource.Play();
    }
}
