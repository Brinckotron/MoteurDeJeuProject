using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] private GameObject inGameMenu;
    [SerializeField] private Slider musicVolume;
    [SerializeField] private Slider soundVolume;
    [SerializeField] private Toggle postProcToggle;
    private bool _isMenuOpen;
    private Camera mainCam;
    public delegate void Volume();
    public static event Volume OnChange;

    private void Start()
    {
        LoadVolumes();
        mainCam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Menu();
        }
    }

    public void Menu()
    {
        inGameMenu.SetActive(!_isMenuOpen);
        _isMenuOpen = !_isMenuOpen;
        if (_isMenuOpen) GameManager.Instance.PauseGame();
        else GameManager.Instance.ResumeGame();
    }

    public void TogglePostProcessing()
    {
        GameManager.Instance.isPostProcessingActive = postProcToggle.isOn;
        if (postProcToggle.isOn) mainCam.GetComponent<PostProcessing>().ChromaticEffectOn();
        else mainCam.GetComponent<PostProcessing>().ChromaticEffectOff();
    }

    public void SoundVolume()
    {
        GameManager.Instance.gameSoundVolume = soundVolume.value;
    }

    public void MusicVolume()
    {
        GameManager.Instance.gameMusicVolume = musicVolume.value;
        if (OnChange != null) OnChange();
        Camera.main.gameObject.GetComponent<AudioSource>().volume = GameManager.Instance.gameMusicVolume;
    }
    
    

    public void LoadVolumes()
    {
        
        Camera.main.gameObject.GetComponent<AudioSource>().volume = GameManager.Instance.gameMusicVolume;
        soundVolume.value = GameManager.Instance.gameSoundVolume;
        musicVolume.value = GameManager.Instance.gameMusicVolume;
    }
}
