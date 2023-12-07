using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    private GameObject _player;
    [SerializeField] private AudioClip clip;
    
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _player.GetComponent<Rigidbody2D>().position = transform.position;
        Camera.main.GetComponent<AudioSource>().clip = clip;
        Camera.main.GetComponent<AudioSource>().Play();
    }
}
