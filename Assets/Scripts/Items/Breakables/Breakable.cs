using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Breakable : MonoBehaviour
{
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip breakSound;
    [SerializeField] protected GameObject breakEffect;
    [SerializeField] protected GameObject smokeEffect;
    [SerializeField] protected GameObject[] loot;
    [SerializeField] protected float volume = 1;
    [SerializeField] protected int lootChance;
    [SerializeField] protected int potentialLootAmount;

    public virtual void BreakSound()
    {
        var soundPoint = Instantiate(audioSource, transform.position, transform.rotation);
        soundPoint.clip = breakSound;
        soundPoint.volume = GameManager.Instance.gameSoundVolume * volume;
        soundPoint.pitch = Random.Range(0.9f, 1.1f);
        soundPoint.Play();
        Destroy(soundPoint, breakSound.length);

    }

    public virtual void RandomLootDrop(int percentage)
    {
        if (Random.Range(1, 101) <= percentage)
        {
            Instantiate(loot[Random.Range(0, loot.Length)], transform.position, transform.rotation);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Instantiate(breakEffect, transform.position, transform.rotation);
        Instantiate(smokeEffect, transform.position, transform.rotation);
        BreakSound();
        for (int i = 1; i == potentialLootAmount; i++)
        {
            RandomLootDrop(lootChance);
        }
        Destroy(gameObject);
    }
}
