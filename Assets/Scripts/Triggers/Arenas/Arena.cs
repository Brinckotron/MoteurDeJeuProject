using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Arena : MonoBehaviour
{
    [SerializeField] private GameObject[] enemiesToWake, gates;
    [SerializeField] private GameObject redMarker;
    [SerializeField] private Transform centerPoint;
    [SerializeField] private float cameraField = 3;
    private List<GameObject> _redMarkers;
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
        ChangeVolume();
        GetComponent<BoxCollider2D>().enabled = false;
        EnemyBehaviour.OnDeath += UpdateArenaCount;
        InGameMenu.OnChange += ChangeVolume;
        StartCoroutine(StartArena());
        _enemyCount = enemiesToWake.Length;
        
    }

    private void OnDisable()
    {
        EnemyBehaviour.OnDeath -= UpdateArenaCount;
        InGameMenu.OnChange -= ChangeVolume;
    }

    private void WakeEnemies()
    {
        foreach (var enemy in enemiesToWake)
        {
            enemy.GetComponent<EnemyBehaviour>().isSleeping = false;
        }
    }

    private IEnumerator StartArena()
    {
        GameManager.Instance.GameState = GameManager.Status.ArenaLoad;
        _player.GetComponentInParent<PlayerController>().Instance.hasEnteredArena = true;
        for (int i = 1; i < 41; i++)
        {
            Camera.main.orthographicSize = Mathf.SmoothStep(_baseCameraField, cameraField, ((float)i/40));
            Camera.main.transform.position =
                new Vector3(Mathf.SmoothStep(_player.transform.position.x, centerPoint.position.x, ((float)i/40)),
                    Mathf.SmoothStep(_player.transform.position.y, centerPoint.position.y, ((float)i/40)), _cameraZ);
            yield return new WaitForSecondsRealtime(0.01f);
        }

        _redMarkers = new List<GameObject>();
        foreach (var enemy in enemiesToWake)
        {
            _redMarkers.Add(Instantiate(redMarker, enemy.transform.position + new Vector3(0, 0.2f, 0), Quaternion.Euler(0, 0, 180), enemy.transform));
        }
        yield return new WaitForSecondsRealtime(1.5f);
        LockGates();
        audioSource.Play();
        yield return new WaitForSecondsRealtime(1.5f);
        for (int i = 1; i < 41; i++)
        {
            Camera.main.orthographicSize = Mathf.SmoothStep(_baseCameraField, cameraField, 1f - ((float)i/40));
            Camera.main.transform.position =
                new Vector3(Mathf.SmoothStep(_player.transform.position.x, centerPoint.position.x, 1f - ((float)i/40)),
                    Mathf.SmoothStep(_player.transform.position.y, centerPoint.position.y, 1 - ((float)i/40)), _cameraZ);
            yield return new WaitForSecondsRealtime(0.01f);
        }
        _player.GetComponentInParent<PlayerController>().Instance.hasEnteredArena = false;
        foreach (var marker in _redMarkers)
        {
            Destroy(marker);
        }
        WakeEnemies();
        GameManager.Instance.GameState = GameManager.Status.Play;
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
        yield return new WaitForSeconds(2f);
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

    private void ChangeVolume()
    {
        audioSource.volume = GameManager.Instance.gameMusicVolume * 0.9f;
    }
}