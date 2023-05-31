using System;
using UnityEngine;

public class IntroLevelManager : MonoBehaviour
{
    private void Start()
    {
        AudioManager.instance.Stop("MainTheme");
    }
}
