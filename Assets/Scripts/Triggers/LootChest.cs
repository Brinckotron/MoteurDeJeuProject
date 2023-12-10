using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class LootChest : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private Sprite openedSprite;
    [SerializeField] private GameObject[] fixedInventory;
    [SerializeField] private GameObject[] randomInventory;
    [SerializeField] private float volume = 1;
    [SerializeField] private bool isRandom;
    [SerializeField] private int randomItemCount;
    [SerializeField] private TMP_Text chestText;
    [SerializeField] private BoxCollider2D coll2D;
    private bool isPlayerInRange;
    private bool isChestOpened;

    private void Update()
    {
        if (GameManager.Instance.GameState == GameManager.Status.Play && Input.GetKeyDown(KeyCode.E) && isPlayerInRange && !isChestOpened)
        {
            OpenSound();
            SpawnLoot();
            GameManager.Instance.chestsOpened++;
            gameObject.GetComponent<SpriteRenderer>().sprite = openedSprite;
            isChestOpened = true;
            isPlayerInRange = false;
            chestText.enabled = false;
            coll2D.enabled = false;
        }
    }

    public void OpenSound()
    {
        var soundPoint = Instantiate(audioSource, transform.position, transform.rotation);
        soundPoint.clip = openSound;
        soundPoint.volume = GameManager.Instance.gameSoundVolume * volume;
        soundPoint.pitch = Random.Range(0.9f, 1.1f);
        soundPoint.Play();
        Destroy(soundPoint, openSound.length);
    }

    private void SpawnLoot()
    {
        if (isRandom)
        {
            for (int i = 0; i < randomItemCount; i++)
            {
                Instantiate(randomInventory[Random.Range(0, randomInventory.Length - 1)], RandomDropPoint(), transform.rotation);
            }
        }
        else
        {
            foreach (var item in fixedInventory)
            {
                Instantiate(item, RandomDropPoint(), transform.rotation);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 9 && !isChestOpened)
        {
            isPlayerInRange = true;
            chestText.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        chestText.enabled = false;
        isPlayerInRange = false;
    }

    public virtual Vector3 RandomDropPoint()
    {
        var point = transform.position + (Vector3)(Random.insideUnitCircle * 0.2f);
        if (point.y < transform.position.y) point.y = transform.position.y;
        if (Physics2D.Raycast(transform.position, Vector2.left, 0.2f, 10).collider != null) point.x = Mathf.Clamp(point.x, 0f, 0.2f);
        if (Physics2D.Raycast(transform.position, Vector2.right, 0.2f, 10).collider != null) point.x = Mathf.Clamp(point.x, -0.2f, 0f);
        return point;
    }
}
