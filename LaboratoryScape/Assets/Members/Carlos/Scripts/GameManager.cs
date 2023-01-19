using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject optionsMenuPanel;

    private void Awake()
    {
        pauseMenuCanvas = GameObject.Find("PauseMenu_Canvas");
        pauseMenuPanel = pauseMenuCanvas.transform.GetChild(0).gameObject;
        optionsMenuPanel = pauseMenuCanvas.transform.GetChild(1).gameObject;
    }

    private void Start()
    {
        Time.timeScale = 1f;
        
        pauseMenuCanvas.SetActive(false);
        pauseMenuPanel.SetActive(true);
        optionsMenuPanel.SetActive(false);
    }

    public void ResumeGame()
    {
        pauseMenuCanvas.SetActive(false);
        pauseMenuPanel.SetActive(true);
        optionsMenuPanel.SetActive(false);
    }

    public void OpenPauseMenu()
    {
        pauseMenuCanvas.SetActive(true);
        pauseMenuPanel.SetActive(true);
        optionsMenuPanel.SetActive(false);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void OpenOptions()
    {
        pauseMenuPanel.SetActive(false);
        optionsMenuPanel.SetActive(true);
    }
}
