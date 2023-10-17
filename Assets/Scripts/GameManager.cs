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
    public int coins, level, xp, xpToNextLevel;
    private Image healthBar, staminaBar, xpBar, healthFrame, staminaFrame, xpFrame, coinsFrame;
    private Text coinsText, lvlText;

    public PlayerController Player;
    private static readonly Color ColorGold = new Color(0.9803922f, 0.7960784f, 0.345098f);

    public void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    public void Initialize(PlayerController player)
    {
        Player = player;
        UI.LoadAssets();
    }

    

    public void LooseHealth(int dmg)
    {
        if (currentHealth == 0) return;
        if (currentHealth <= dmg)
        {
            Death();
            Player.Instance.Death();
            currentHealth = 0;
        }
        else
        {
            currentHealth -= dmg;
        }

        UI.Health.Update();
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

    public void GainCoins(int amount)
    {
        coins += amount;
        UI.Coins.Update();
        StartCoroutine(UI.Coins.Flash(Color.white));
    }

    public void Death()
    {
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
                        Instance.healthBar = image;
                        break;
                    case "StaminaFiller":
                        Instance.staminaBar = image;
                        break;
                    case "XPFiller":
                        Instance.xpBar = image;
                        break;
                    case "HealthFrame":
                        Instance.healthFrame = image;
                        break;
                    case "StaminaFrame":
                        Instance.staminaFrame = image;
                        break;
                    case "XPFrame":
                        Instance.xpFrame = image;
                        break;
                    case "CoinsFrame":
                        Instance.coinsFrame = image;
                        break;
                }
            }

            var textArray = FindObjectsOfType<Text>(true);
            foreach (var text in textArray)
            {
                switch (text.name)
                {
                    case "LvlText":
                        Instance.lvlText = text;
                        break;
                    case "CoinsText":
                        Instance.coinsText = text;
                        break;
                }
            }

            UI.UpdateAllBars();
        }
        public static void UpdateAllBars()
        {
            Instance.healthBar.fillAmount = (Instance.currentHealth / Instance.maxHealth);
            Instance.staminaBar.fillAmount = (Instance.currentStamina / Instance.maxStamina);
            Instance.xpBar.fillAmount = ((float)Instance.xp / (float)Instance.xpToNextLevel);
            Instance.lvlText.text = Instance.level.ToString();
            Instance.coinsText.text = Instance.coins.ToString();
        }

        public static class Health
        {
            public static void Update()
            {
                Instance.healthBar.fillAmount = (Instance.currentHealth / Instance.maxHealth);
            }

            public static IEnumerator Flash(Color color)
            {
                for (var i = 0; i <= 5; i++)
                {
                    yield return new WaitForSeconds(0.04f);
                    Instance.healthFrame.color = Color.Lerp(color, ColorGold, (0.2f * i));
                }
            }
        }

        public static class Stamina
        {
            public static void Update()
            {
                Instance.staminaBar.fillAmount = (Instance.currentStamina / Instance.maxStamina);
            }
            
            public static IEnumerator Flash(Color color)
            {
                for (var i = 0; i <= 5; i++)
                {
                    yield return new WaitForSeconds(0.04f);
                    Instance.staminaFrame.color = Color.Lerp(color, ColorGold, (0.2f * i));
                }
            }
        }

        public static class Xp
        {
            public static void UpdateXp()
            {
                Instance.xpBar.fillAmount = ((float)Instance.xp / (float)Instance.xpToNextLevel);
            }

            public static void UpdateLvl()
            {
                Instance.lvlText.text = Instance.level.ToString();
            }
            
            public static IEnumerator Flash(Color color)
            {
                for (var i = 0; i <= 5; i++)
                {
                    yield return new WaitForSeconds(0.04f);
                    Instance.xpFrame.color = Color.Lerp(color, ColorGold, (0.2f * i));
                }
            }
        }

        public static class Coins
        {
            public static void Update()
            {
                Instance.coinsText.text = Instance.coins.ToString();
            }
            
            public static IEnumerator Flash(Color color)
            {
                for (var i = 0; i <= 5; i++)
                {
                    yield return new WaitForSeconds(0.04f);
                    Instance.coinsFrame.color = Color.Lerp(color, ColorGold, (0.2f * i));
                }
            }
        }
    }
}