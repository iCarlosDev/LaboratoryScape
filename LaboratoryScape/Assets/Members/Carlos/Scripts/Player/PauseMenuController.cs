using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private bool shouldPause;

    public bool ShouldPause => shouldPause;

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
    }

    private void Update()
    {
        PauseMenuControl();
    }

    private void PauseMenuControl()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            shouldPause = !shouldPause;

            if (shouldPause)
            {
                OpenPauseMenu();
            }
            else
            {
                ClosePauseMenu();
            }
        }
    }
    
    private void OpenPauseMenu()
    {
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        gameManager.OpenPauseMenu();
    }
    
    public void ClosePauseMenu()
    {
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        shouldPause = false;
        gameManager.ResumeGame();
    }
}
