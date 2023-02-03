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
            Possess();
        }
        else
        {
            PossessCooldown();
        }
    }
    
    /// <summary>
    /// Método para activar la posesión de un enemigo;
    /// </summary>
    private void Possess()
    {
        if (Input.GetKeyDown(KeyCode.F) && canPossess)
        {
            possessCooldown = 5f;
            imPossessing = true;
        }
    }

    /// <summary>
    /// Método para activar el cooldown de posesión cuando hemos poseido a un enemigo;
    /// </summary>
    private void PossessCooldown()
    {
        possessCooldown -= Time.deltaTime;

        if (possessCooldown <= 0)
        {
            haveCooldown = false;
        }
    }

   
    /// <summary>
    /// Si un enemigo entra en nuestro rango de posesión podremos poseerle; 
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            canPossess = true;
        }
    }

    /// <summary>
    /// Si un enemigo sale de nuestro rango de posesión dejaremos de poder poseerle; 
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            canPossess = false;
        }
    }
}
