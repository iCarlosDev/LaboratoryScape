using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPossess : MonoBehaviour
{
    [Header("--- POSESSION ---")]
    [SerializeField] private bool canPossess;
    [SerializeField] private bool imPossessing;
    
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

    /////////////////////////////////////////


    private void Update()
    {
        if (canPossess)
        {
            Possess();
        }
    }
    
    private void Possess()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            imPossessing = true;
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
