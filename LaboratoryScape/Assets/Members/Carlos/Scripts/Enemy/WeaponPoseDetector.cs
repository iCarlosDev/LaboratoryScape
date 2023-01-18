using System;
using Demo.Scripts.Runtime;
using UnityEngine;

public class WeaponPoseDetector : MonoBehaviour
{
    //Variables
    [SerializeField] private bool isBlocked;
    [SerializeField] private EnemyScriptsStorage enemyScriptsStorage;

    public bool IsBlocked
    {
        get => isBlocked;
        set => isBlocked = value;
    }

    private void Awake()
    {
        enemyScriptsStorage = GetComponentInParent<EnemyScriptsStorage>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            enemyScriptsStorage.FPSController.WeaponBlockFlag = true;
            enemyScriptsStorage.FPSController.aiming = false;
            isBlocked = true;
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
