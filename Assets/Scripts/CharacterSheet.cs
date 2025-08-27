using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSheet : MonoBehaviour
{
   private GameManager _gm;
   [SerializeField] private GameObject charSheet;
   [SerializeField] private Image character;
   [SerializeField] private Sprite[] knightImages;
   [SerializeField] private TMP_Text knightName;
   [SerializeField] private TMP_Text knightLevel;
   [SerializeField] private TMP_Text knightXP;
   [SerializeField] private TMP_Text unspentPoints;
   [SerializeField] private TMP_Text hp;
   [SerializeField] private TMP_Text stamina;
   [SerializeField] private TMP_Text daggerAmount;
   [SerializeField] private TMP_Text fireFlaskAmount;
   [SerializeField] private TMP_Text iceFlaskAmount;

   private bool _isCharSheetOpen;


   private void Start()
   {
      LoadAssets();
   }

   private void Update()
   {
      if (_gm.GameState is not (GameManager.Status.Paused or GameManager.Status.ArenaLoad) &&
          Input.GetKeyDown(KeyCode.C))
      {
         OpenClose(false);
      }
      
      if (_isCharSheetOpen && Input.GetKeyDown(KeyCode.Escape)) OpenClose(true); 
   }

   public void OpenClose(bool isEscapePressed)
   {
      charSheet.SetActive(!_isCharSheetOpen);
      LoadAssets();
      _isCharSheetOpen = !_isCharSheetOpen;
      if (_isCharSheetOpen) _gm.PauseGame();
      else if (!isEscapePressed) _gm.ResumeGame();
   }

   private void LoadAssets()
   {
      _gm = GameManager.Instance;
      character.sprite = knightImages[_gm.knight];
      knightName.text = _gm.playerName;
      knightLevel.text = _gm.level.ToString();
      knightXP.text = ($"XP : {_gm.xp.ToString()} / {_gm.xpToNextLevel.ToString()}");
      unspentPoints.text = _gm.unspentLevels.ToString();
      hp.text = _gm.maxHealth.ToString();
      stamina.text = _gm.maxStamina.ToString();
      daggerAmount.text = _gm.daggerAmount.ToString();
      fireFlaskAmount.text = _gm.fireFlaskAmount.ToString();
      iceFlaskAmount.text = _gm.iceFlaskAmount.ToString();
   }
   
}
