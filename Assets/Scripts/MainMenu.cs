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
    [SerializeField] private TMP_Text descriptiveText;
    [SerializeField] private GameObject newGameWindow;
    [SerializeField] private AudioSource music;
    [SerializeField] private Slider musicVolume, soundVolume;
    private readonly string[] _knightNames = new[] { "Edrik", "Lance", "Korbin" };
    private readonly string[] _knightDesc = new[]
    {
        "Edrik is a well rounded warrior, as agile and resilient as any knight.",
        "Lance is a more robust, stiffer kind of knight, tougher but quickly fatigued.",
        "Korbin lacks the physical endurance of his peers, but he makes up for it with his impressive stamina."
    };
        
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
        SceneManager.LoadScene("Prime", LoadSceneMode.Single);
        SceneManager.LoadScene("Haven", LoadSceneMode.Additive);
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
        descriptiveText.text = _knightDesc[GameManager.Instance.knight];
    }


}
