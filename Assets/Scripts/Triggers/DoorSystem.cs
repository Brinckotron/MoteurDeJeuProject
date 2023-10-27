using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorSystem : MonoBehaviour
{
    [SerializeField] private GameObject doorOpen;
    [SerializeField] private GameObject doorClosed;
    [SerializeField] private string destination;
    [SerializeField] private TMP_Text doorText;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] doorSounds;
    private bool _canTravel;
    private bool _isTravelling;


    private void Update()
    {
        if (_canTravel && Input.GetKeyDown(KeyCode.E))
        {
            _isTravelling = true;
            Travel();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
            doorClosed.SetActive(false);
            doorOpen.SetActive(true);
            doorText.enabled = true;
            _canTravel = true;
            PlaySound(doorSounds[0]);
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!_isTravelling)
        {
            doorClosed.SetActive(true);
            doorOpen.SetActive(false);
            doorText.enabled = false;
            _canTravel = false;
            PlaySound(doorSounds[1]);
        }
    }

    private void Travel()
    {
        SceneManager.LoadScene(destination);
    }
    
    public virtual void PlaySound(AudioClip clip)
    {
        var soundPoint = Instantiate(audioSource, transform.position, transform.rotation);
        soundPoint.clip = clip;
        soundPoint.volume = GameManager.Instance.gameSoundVolume * 0.75f;
        soundPoint.Play();
        Destroy(soundPoint, clip.length);
    }

}
