using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class DeathScreen : MonoBehaviour
{
    [SerializeField] private GameObject progress;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text gold;
    [SerializeField] private TMP_Text chests;
    [SerializeField] private TMP_Text level;
    [SerializeField] private TMP_Text xp;
    [SerializeField] private TMP_Text kills;
    [SerializeField] private TMP_Text deadText;
    [SerializeField] private TMP_Text lostText;
    [SerializeField] private GameObject newRun;
    [SerializeField] private GameObject mainMenu;
    private List<TMP_Text> textList;
    

    public void Initialize()
    {
        textList = new List<TMP_Text>();
        nameText.text = GameManager.Instance.playerName;
        gold.text = GameManager.Instance.coins.ToString();
        chests.text = GameManager.Instance.chestsOpened.ToString();
        level.text = GameManager.Instance.level.ToString();
        xp.text = GameManager.Instance.accumulatedXP.ToString();
        kills.text = GameManager.Instance.kills.ToString();
        GetComponent<Image>().color = Color.clear;
        deadText.color = Color.clear;
        nameText.color = Color.clear;
        lostText.color = Color.clear;
        foreach (var text in progress.GetComponentsInChildren<TMP_Text>())
        {
            text.color = Color.clear;
            textList.Add(text);
        }

        StartCoroutine(Reveal());
    }

    private IEnumerator Reveal()
    {
        yield return new WaitForSecondsRealtime(1f);
        for (int i = 0; i < 21; i++)
        {
            yield return new WaitForSecondsRealtime(0.20f);
            GetComponent<Image>().color = new Color(0, 0, 0, Mathf.Lerp(0, 1, (float)i/20));
        }
        
        for (int i = 0; i < 21; i++)
        {
            Color tempColor = new Color(1, 1, 1, Mathf.SmoothStep(0, 1, (float)i/20));
            yield return new WaitForSecondsRealtime(0.20f);
            deadText.color = tempColor;
            nameText.color = tempColor;
            lostText.color = tempColor;


        }
        
        for (int i = 0; i < 21; i++)
        {
            yield return new WaitForSecondsRealtime(0.20f);
            foreach (var tmpText in textList)
            {
                tmpText.color = new Color(1, 1, 1, Mathf.Lerp(0, 1, (float)i/20));
            }
            
        }
        
        newRun.SetActive(true);
        mainMenu.SetActive(true);
        
    }

    public void NewRun()
    {
        
    }

    public void ReturnToMainMenu()
    {
    }
}
