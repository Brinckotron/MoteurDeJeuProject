using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalSystem : MonoBehaviour
{
    [SerializeField] private TMP_Text portalText;
    [SerializeField] private string destination;
    private bool _canTravel;

    public void ChangeDestination(string newDestination)
    {
        destination = newDestination;
    }

    private void Update()
    {
        if (_canTravel && Input.GetKeyDown(KeyCode.E)) Travel();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        portalText.enabled = true;
        _canTravel = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        portalText.enabled = false;
        _canTravel = false;
    }
    private void Travel()
    {
        SceneManager.LoadScene(destination);
    }
}
