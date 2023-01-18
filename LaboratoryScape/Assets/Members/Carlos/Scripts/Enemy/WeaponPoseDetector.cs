using System;
using System.Collections.Generic;
using Demo.Scripts.Runtime;
using UnityEngine;

public class WeaponPoseDetector : MonoBehaviour
{
    //Variables
    [SerializeField] private EnemyScriptsStorage enemyScriptsStorage;
    [SerializeField] private bool isBlocked;

    //GETTERS && SETTERS//
    public bool IsBlocked
    {
        get => isBlocked;
        set => isBlocked = value;
    }

    ////////////////////////////////////

    private void Awake()
    {
        enemyScriptsStorage = GetComponentInParent<EnemyScriptsStorage>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            enemyScriptsStorage.FPSController.WeaponBlockFlag = true;
            isBlocked = true;
            //enemyScriptsStorage.FPSController.aiming = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            isBlocked = false;
        }
    }
}
