using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private void Awake()
    {
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            string currentScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene("Prime");
            SceneManager.LoadSceneAsync(currentScene, LoadSceneMode.Additive);
        }
    }
}
