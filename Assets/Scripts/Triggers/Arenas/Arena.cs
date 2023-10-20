using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Arena : MonoBehaviour
{
    [SerializeField] private GameObject[] enemiesToWake, gates;
    [SerializeField] private Transform centerPoint;
    [SerializeField] private float cameraField = 3;
    private AudioSource audioSource;
    private GameObject _player;
    private float _enemyCount, _baseCameraField = 3f, _cameraZ;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 9)
        {
            _player = other.gameObject;
            _cameraZ = Camera.main.transform.position.z;
            Activate();
        }
    }

    public virtual void Activate()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        GetComponent<BoxCollider2D>().enabled = false;
        EnemyBehaviour.OnDeath += UpdateArenaCount;
        LockGates();
        StartCoroutine(ShowArena());
        WakeEnemies();
        
        _enemyCount = enemiesToWake.Length;
        
    }

    private void OnDisable()
    {
        EnemyBehaviour.OnDeath -= UpdateArenaCount;
    }

    private void WakeEnemies()
    {
        foreach (var enemy in enemiesToWake)
        {
            enemy.GetComponent<EnemyBehaviour>().isSleeping = false;
        }
    }

    private IEnumerator ShowArena()
    {
        _player.GetComponentInParent<PlayerController>().Instance.hasEnteredArena = true;
        Time.timeScale = 0.1f;
        for (int i = 1; i < 41; i++)
        {
            Camera.main.orthographicSize = Mathf.SmoothStep(_baseCameraField, cameraField, ((float)i/40));
            Camera.main.transform.position =
                new Vector3(Mathf.SmoothStep(_player.transform.position.x, centerPoint.position.x, ((float)i/40)),
                    Mathf.SmoothStep(_player.transform.position.y, centerPoint.position.y, ((float)i/40)), _cameraZ);
            yield return new WaitForSecondsRealtime(0.01f);
        }

        yield return new WaitForSecondsRealtime(3f);
        for (int i = 1; i < 41; i++)
        {
            Camera.main.orthographicSize = Mathf.SmoothStep(_baseCameraField, cameraField, 1f - ((float)i/40));
            Camera.main.transform.position =
                new Vector3(Mathf.SmoothStep(_player.transform.position.x, centerPoint.position.x, 1f - ((float)i/40)),
                    Mathf.SmoothStep(_player.transform.position.y, centerPoint.position.y, 1 - ((float)i/40)), _cameraZ);
            yield return new WaitForSecondsRealtime(0.01f);
        }
        _player.GetComponentInParent<PlayerController>().Instance.hasEnteredArena = false;

        Time.timeScale = 1f;
    }

    private void LockGates()
    {
        foreach (var gate in gates)
        {
            gate.SetActive(true);
        }
    }

    private void UnlockGates()
    {
        foreach (var gate in gates)
        {
            gate.SetActive(false);
        }
    }

    private IEnumerator DestroyArena()
    {
        for (int i = 0; i < 7; i++)
        {
            audioSource.volume -= 0.05f;
            yield return new WaitForSeconds(0.25f);
        }

        Destroy(transform.parent.gameObject);
    }

    private void UpdateArenaCount()
    {
        _enemyCount--;
        if (_enemyCount == 0)
        {
            UnlockGates();
            StartCoroutine(DestroyArena());
        }
    }
}