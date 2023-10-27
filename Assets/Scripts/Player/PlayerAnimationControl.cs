using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationControl : MonoBehaviour
{
    public static PlayerAnimationControl instance;
    private Animator anim;
    public string currentState;
    [SerializeField] private AudioSource audioMove, audioAction;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;
        anim.Play(newState);
        currentState = newState;
    }

    public void PlaySoundMove(AudioClip clip)
    {
        audioMove.volume = GameManager.Instance.gameSoundVolume;
        audioMove.clip = clip;
        audioMove.Play();
    }
    public void PlaySoundAction(AudioClip clip)
    {
        audioAction.volume = GameManager.Instance.gameSoundVolume;
        audioAction.clip = clip;
        audioAction.Play();
    }
}