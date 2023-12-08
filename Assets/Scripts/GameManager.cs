using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Singleton

    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("GameManager is NULL");
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance) Destroy(gameObject);
        else _instance = this;
        DontDestroyOnLoad(this);
    }

    #endregion

    public float maxHealth, currentHealth, maxStamina, currentStamina;
    public float gameMusicVolume = 0.5f;
    public float gameSoundVolume = 0.5f;
    public string playerName = "Edrik";
    public int knight = 0;
    public int coins, level, xp, xpToNextLevel;
    private Image _healthBar, _staminaBar, _xpBar, _healthFrame, _staminaFrame, _xpFrame, _coinsFrame;
    private Text _coinsText, _lvlText;
    public bool isPostProcessingActive = true;
    public PlayerController player;
    private static readonly Color ColorGold = new Color(0.9803922f, 0.7960784f, 0.345098f);
    private Camera mainCam;
    private GameObject deathScreen;
    public enum Status
    {
        Play,
        Paused,
        ArenaLoad
    }
    public Status gameState;
    public Status GameState
    {
        get { return gameState; }
        set { gameState = value; }
    }

    public void Initialize(PlayerController playerController)
    {
        switch (knight)
        {
            case 0:
                maxHealth = 100;
                maxStamina = 100;
                break;
            case 1:
                maxHealth = 120;
                maxStamina = 80;
                break;
            case 2:
                maxHealth = 80;
                maxStamina = 120;
                break;
        }
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        this.player = playerController;
        deathScreen = GameObject.Find("DeathScreen");
        deathScreen.SetActive(false);
        UI.LoadAssets();
        GameState = Status.Play;
        mainCam = Camera.main;
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        gameState = Status.Paused;
        if (isPostProcessingActive) mainCam.GetComponent<PostProcessing>().ChromaticEffectOn();
    }
    
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        gameState = Status.Play;
        if (isPostProcessingActive) mainCam.GetComponent<PostProcessing>().ChromaticEffectOff();
    }

    public void GainGold(int amount)
    {
        coins += amount;
        UI.Coins.Update();
        StartCoroutine(UI.Coins.Flash(amount > 0 ? Color.green: Color.red));
    }

    public void LooseHealth(int dmg)
    {
        if (currentHealth == 0) return;
        if (currentHealth <= dmg)
        {
            Death();
            player.Instance.Death();
            currentHealth = 0;
        }
        else
        {
            currentHealth -= dmg;
        }

        UI.Health.Update();
        if (isPostProcessingActive) mainCam.GetComponent<PostProcessing>().BloodEffect();
        StartCoroutine(UI.Health.Flash(Color.red));
    }
    
    public void GainHealth(int healthValue)
    {
        if (currentHealth == maxHealth) return;
        if (maxHealth - currentHealth <= healthValue)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += healthValue;
        }

        UI.Health.Update();
        StartCoroutine(UI.Health.Flash(Color.green));
    }

    public void UseStamina(int stamCost)
    {
        if (currentStamina >= stamCost)
        {
            currentStamina -= stamCost;
            UI.Stamina.Update();
            StartCoroutine(UI.Stamina.Flash(Color.white));
        }
        else
        {
            StartCoroutine(UI.Stamina.Flash(Color.red));
        }
        
    }

    public void StaminaRegenTick(float amount)
    {
        if ((currentStamina + amount) > maxStamina)
        {
            currentStamina = maxStamina;
            StartCoroutine(UI.Stamina.Flash(Color.green));
        }
        else currentStamina += amount;
        UI.Stamina.Update();
    }

    public void GainXp(int amount)
    {
        xp += amount;
        if (xp >= xpToNextLevel)
        {
            LvlUp();
            xp -= xpToNextLevel;
        }

        UI.Xp.UpdateXp();
        StartCoroutine(UI.Xp.Flash(Color.white));
    }

    public void LvlUp()
    {
        level++;
        
        UI.Xp.UpdateLvl();
    }

    public void Death()
    {
        deathScreen.SetActive(true);
        deathScreen.GetComponent<DeathScreen>().Initialize();
    }
    public static class UI
    {
        public static void LoadAssets()
        {
            var imageArray = FindObjectsOfType<Image>(true);
            foreach (var image in imageArray)
            {
                switch (image.name)
                {
                    case "HealthFiller":
                        Instance._healthBar = image;
                        break;
                    case "StaminaFiller":
                        Instance._staminaBar = image;
                        break;
                    case "XPFiller":
                        Instance._xpBar = image;
                        break;
                    case "HealthFrame":
                        Instance._healthFrame = image;
                        break;
                    case "StaminaFrame":
                        Instance._staminaFrame = image;
                        break;
                    case "XPFrame":
                        Instance._xpFrame = image;
                        break;
                    case "CoinsFrame":
                        Instance._coinsFrame = image;
                        break;
                }
            }

            var textArray = FindObjectsOfType<Text>(true);
            foreach (var text in textArray)
            {
                switch (text.name)
                {
                    case "LvlText":
                        Instance._lvlText = text;
                        break;
                    case "CoinsText":
                        Instance._coinsText = text;
                        break;
                }
            }
            
            UpdateAllBars();
        }
        public static void UpdateAllBars()
        {
            Instance._healthBar.fillAmount = (Instance.currentHealth / Instance.maxHealth);
            Instance._staminaBar.fillAmount = (Instance.currentStamina / Instance.maxStamina);
            Instance._xpBar.fillAmount = ((float)Instance.xp / (float)Instance.xpToNextLevel);
            Instance._lvlText.text = Instance.level.ToString();
            Instance._coinsText.text = Instance.coins.ToString();
        }

        
        

            
        

        public static class Health
        {
            public static void Update()
            {
                Instance._healthBar.fillAmount = (Instance.currentHealth / Instance.maxHealth);
            }

            public static IEnumerator Flash(Color color)
            {
                for (var i = 0; i <= 5; i++)
                {
                    yield return new WaitForSeconds(0.04f);
                    Instance._healthFrame.color = Color.Lerp(color, ColorGold, (0.2f * i));
                }
            }
        }

        public static class Stamina
        {
            public static void Update()
            {
                Instance._staminaBar.fillAmount = (Instance.currentStamina / Instance.maxStamina);
            }
            
            public static IEnumerator Flash(Color color)
            {
                for (var i = 0; i <= 5; i++)
                {
                    yield return new WaitForSeconds(0.04f);
                    Instance._staminaFrame.color = Color.Lerp(color, ColorGold, (0.2f * i));
                }
            }
        }

        public static class Xp
        {
            public static void UpdateXp()
            {
                Instance._xpBar.fillAmount = ((float)Instance.xp / (float)Instance.xpToNextLevel);
            }

            public static void UpdateLvl()
            {
                Instance._lvlText.text = Instance.level.ToString();
            }
            
            public static IEnumerator Flash(Color color)
            {
                for (var i = 0; i <= 5; i++)
                {
                    yield return new WaitForSeconds(0.04f);
                    Instance._xpFrame.color = Color.Lerp(color, ColorGold, (0.2f * i));
                }
            }
        }

        public static class Coins
        {
            public static void Update()
            {
                Instance._coinsText.text = Instance.coins.ToString();
            }
            
            public static IEnumerator Flash(Color color)
            {
                for (var i = 0; i <= 5; i++)
                {
                    yield return new WaitForSeconds(0.04f);
                    Instance._coinsFrame.color = Color.Lerp(color, ColorGold, (0.2f * i));
                }
            }
        }
    }
}