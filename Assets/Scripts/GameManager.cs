using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


/*
Game manager semble être utilisé un peu trop comme un fourre tout.
Imo, le game manager doit être utiliser pour contrôller des éléments high level du jeu:
- Le state du jeu (paused, running, etc.)
- Gérer le loading/déloading des scènes (un scene manager pourrait être utilisé si il y a des transitions complexes)
- Gérer des settings de préférence ou global
Tout le reste devrait être dans des systèmes, manager ou entités.
Exemples:
- Les datas de health/stamina/level/xp pourraient être attaché directement au player
- Le UI pourrait avoir son propre manager et listen a des events sur le player (healthchanged, levelchanged, staminachanged, etc)
- Un spawn manager pour gérer les spawns
De cette façon ton gamemanager pourrais par exemple se limiter a listen a un event sur ton player qui signale la mort de celui-ci et transitionner vers un nouveau state
*/
public class GameManager : MonoBehaviour
{
    #region Singleton
    /*
    Pour te faciliter la vie tu peux utiliser une base class pour tes singletons
    Exemple:
    ----------
    #GameManager.cs
    public class GameManager : Singleton<GameManager> 
    ----------
    #Singleton.cs    
    using UnityEngine;

    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static bool _applicationIsQuitting = false;
        private static readonly object Lock = new object();

        public static T Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;
                
                _instance = FindAnyObjectByType<T>();

                if (!_instance && !_applicationIsQuitting)
                {
                    lock (Lock)
                    {
                        GameObject singletonObject = new GameObject();
                        _instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).ToString();

                        DontDestroyOnLoad(singletonObject);
                    }
                }
                return _instance;
            }
        }

        private void OnApplicationQuit()
        {
            _instance = null;
            Destroy(gameObject);
            _applicationIsQuitting = true;
        }
    }
    */
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
    [SerializeField] private Image healthBar, staminaBar, xpBar, healthFrame, staminaFrame, xpFrame, coinsFrame;
    [SerializeField] private Text coinsText, lvlText;

    public PlayerController Player;
    [SerializeField] private GameObject goblin;
    [SerializeField] private Transform spawner;
    
    //Dépendamment de la taille du projet ça peut être intéressant d'avoir un scriptable object avec ton theme ou
    //une constant class pour éviter de devoir changer des variables un peu partout
    private static readonly Color ColorGold = new Color(0.9803922f, 0.7960784f, 0.345098f);

    public void Start()
    {
        /*if(GameObject.FindWithTag("Player").GetComponent<PlayerController>() != null) Player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        UI.UpdateAllBars();*/
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) Instantiate(goblin, spawner);
    }

    
    /*
    Dans la vie, c'est un atout d'être paresseux quand tu es prog :P
    Si tu peux éviter de réécrire des lignes de code pour une logique similaire, do it.
    Exemple:
    LooseHealth et GainHealth pourraient êtres une seule fonction puisque'elles sont très similaire:
    public void ModifyHealth(int value)
    {
        if ((currentHealth == 0 && value < 0) || (currentHealth == maxHealth && value > 0)) return;

        currentHealth += value;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth == 0)
        {
            Death();
            Player.Instance.Death();
        }

        UI.Health.Update();
        Color flashColor = (value < 0) ? Color.red : Color.green;
        StartCoroutine(UI.Health.Flash(flashColor));
    }
    */
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
            //Je te suggère d'éviter des mots réservés même si le contexte est différent. Je ne sais pas ce que les
            //meilleurs pratiques disent à ce sujet mais c'est ma pref en tout cas
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