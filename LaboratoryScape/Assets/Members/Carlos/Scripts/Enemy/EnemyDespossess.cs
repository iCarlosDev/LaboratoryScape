using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDespossess : MonoBehaviour
{
    //Variables
    [Header("--- DESPOSSESS ---")] 
    [SerializeField] private float timeRemaining;
    [SerializeField] private bool shouldSuicide;
    
    
    //GETTERS & SETTERS//
    public bool ShouldSuicide => shouldSuicide;
    
    //////////////////////////////////////////

    private void Update()
    {
        Suicide();
    }

    private void Start()
    {
        timeRemaining = 3000f;
    }

    private void Suicide()
    {
        timeRemaining -= Time.deltaTime;
        
        if (Input.GetKeyDown(KeyCode.F) || timeRemaining <= 0)
        {
            shouldSuicide = true;
        }
    }
}
