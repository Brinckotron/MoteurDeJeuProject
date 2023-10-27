using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSheet : MonoBehaviour
{
   [SerializeField] private GameObject charSheet;
   [SerializeField] private Image character;
   [SerializeField] private Sprite[] knightImages;
   [SerializeField] private TMP_Text knightName;

   private bool _isCharSheetOpen;
   private string[] _knightNames = new string[] { "Edrik", "Lance", "Korbin" };


   private void Start()
   {
      LoadAssets();
   }

   private void Update()
   {
      if (GameManager.Instance.GameState is not (GameManager.Status.Paused or GameManager.Status.ArenaLoad) &&
          Input.GetKeyDown(KeyCode.C))
      {
         OpenClose();
      }
      
      if (_isCharSheetOpen && Input.GetKeyDown(KeyCode.Escape)) OpenClose(); 
   }

   public void OpenClose()
   {
      charSheet.SetActive(!_isCharSheetOpen);
      _isCharSheetOpen = !_isCharSheetOpen;
      if (_isCharSheetOpen) GameManager.Instance.PauseGame();
      else GameManager.Instance.ResumeGame();
   }

   private void LoadAssets()
   {
      character.sprite = knightImages[GameManager.Instance.knight];
      knightName.text = GameManager.Instance.playerName;
   }
}
