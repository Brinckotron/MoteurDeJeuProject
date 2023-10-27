using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Animator character;
    [SerializeField] private TMP_Text warning;
    [SerializeField] private TMP_InputField knightName;
    [SerializeField] private GameObject newGameWindow;
    [SerializeField] private AudioSource music;
    [SerializeField] private Slider musicVolume, soundVolume;
    private string[] _knightNames = new string[] { "Edrik", "Lance", "Korbin" };

    private void Start()
    {
        MusicVolume();
        SoundVolume();
    }

    public void NewGame()
    {
        newGameWindow.SetActive(true);
    }

    public void CloseWindow()
    {
        newGameWindow.SetActive(false);
    }

    public void Continue()
    {
    }

    public void SoundVolume()
    {
        GameManager.Instance.gameSoundVolume = soundVolume.value;
    }

    public void MusicVolume()
    {
        GameManager.Instance.gameMusicVolume = musicVolume.value;
        music.volume = musicVolume.value;
    }

    public void StartButton()
    {
        GameManager.Instance.playerName = knightName.text;
        SceneManager.LoadScene("Haven");
    }

    public void NextCharacter()
    {
        if (GameManager.Instance.knight == 2) GameManager.Instance.knight = 0;
        else GameManager.Instance.knight++;
        UpdateCharacter();
    }

    public void PreviousCharacter()
    {
        if (GameManager.Instance.knight == 0) GameManager.Instance.knight = 2;
        else GameManager.Instance.knight--;
        UpdateCharacter();
    }

    public void UpdateCharacter()
    {
        character.Play(_knightNames[GameManager.Instance.knight]);
        knightName.text = _knightNames[GameManager.Instance.knight];
    }


}
