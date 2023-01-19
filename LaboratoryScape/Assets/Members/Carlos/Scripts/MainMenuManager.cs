using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject MainMenuCanvas;
    [SerializeField] private GameObject OptionsCanvas;
    [SerializeField] private GameObject CreditsCanvas;
    
    [SerializeField] private EventSystem eventSystem;

    private void Awake()
    {
        MainMenuCanvas = GameObject.Find("--- CANVAS ---").transform.GetChild(0).gameObject;
        OptionsCanvas = GameObject.Find("--- CANVAS ---").transform.GetChild(1).gameObject;
        CreditsCanvas = GameObject.Find("--- CANVAS ---").transform.GetChild(2).gameObject;

        eventSystem = FindObjectOfType<EventSystem>();
    }

    private void Start()
    {
        MainMenuCanvas.SetActive(true);
        OptionsCanvas.SetActive(false);
        CreditsCanvas.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenOptions()
    {
        MainMenuCanvas.SetActive(false);
        OptionsCanvas.SetActive(true);
    }

    public void OpenCredits()
    {
        MainMenuCanvas.SetActive(false);
        CreditsCanvas.SetActive(true);
    }

    public void GoMainMenu()
    {
        OptionsCanvas.SetActive(false);
        CreditsCanvas.SetActive(false);
        MainMenuCanvas.SetActive(true);
    }
}
