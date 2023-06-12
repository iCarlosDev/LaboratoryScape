using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkipEnding : MonoBehaviour
{
    [SerializeField] private GameObject enableButtonUI;
    private bool skipIntro = false;

    private void Awake()
    {
        enableButtonUI.SetActive(false);
    }

    private void Update()
    {
        if (Input.anyKey && !enableButtonUI.active)
        {
           enableButtonUI.SetActive(true);
           StartCoroutine(SkipIntroCoroutine());
        }
        if (Input.anyKey && skipIntro)
        {
            SceneManager.LoadScene(0);
        }
    }
    IEnumerator SkipIntroCoroutine()
    {
        // suspend execution for 5 seconds
        yield return new WaitForSeconds(0.5f);
        skipIntro = true;
    }
}
