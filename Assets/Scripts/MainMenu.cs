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
    [SerializeField] private Scene firstScene;
    private int _chosenKnight = 0;
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
    }

    public void MusicVolume()
    {
        music.volume = musicVolume.value;
    }

    public void StartButton()
    {
        SceneManager.LoadScene("Forest");
    }

    public void NextCharacter()
    {
        if (_chosenKnight == 2) _chosenKnight = 0;
        else _chosenKnight++;
        UpdateCharacter();
    }

    public void PreviousCharacter()
    {
        if (_chosenKnight == 0) _chosenKnight = 2;
        else _chosenKnight--;
        UpdateCharacter();
    }

    public void UpdateCharacter()
    {
        character.Play(_knightNames[_chosenKnight]);
        knightName.text = _knightNames[_chosenKnight];
    }


}
