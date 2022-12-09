using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPossess : MonoBehaviour
{
    [Header("--- POSESSION ---")]
    [SerializeField] private bool canPossess;
    [SerializeField] private bool imPossessing;
    [SerializeField] private bool haveCooldown;
    [SerializeField] private float possessCooldown;
    
    //GETTERS & SETTERS//
    public bool CanPossess
    {
        get => canPossess;
        set => canPossess = value;
    }
    public bool ImPossessing
    {
        get => imPossessing;
        set => imPossessing = value;
    }
    public bool HaveCooldown
    {
        get => haveCooldown;
        set => haveCooldown = value;
    }

    /////////////////////////////////////////


    private void Update()
    {
        if (!haveCooldown)
        {
            if (canPossess)
            {
                Possess();
            }
        }
        else
        {
            PossessCooldown();
        }
    }
    
    private void Possess()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            possessCooldown = 5f;
            imPossessing = true;
        }
    }

    private void PossessCooldown()
    {
        possessCooldown -= Time.deltaTime;

        if (possessCooldown <= 0)
        {
            haveCooldown = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            canPossess = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            canPossess = false;
        }
    }
}
