using System;
using System.Collections;
using System.Collections.Generic;
using Demo.Scripts;
using Kinemation.FPSFramework.Runtime.Core;
using Kinemation.FPSFramework.Runtime.Layers;
using UnityEngine;

public class EnemyDespossess : MonoBehaviour
{
    //Variables
    [Header("--- DESPOSSESS ---")] 
    [SerializeField] private float timeRemaining;
    [SerializeField] private bool shouldSuicide;
    
    [Header("--- ENEMY SCRIPTS STORAGE ---")]
    [SerializeField] private BlendingLayer blendingLayer;
    [SerializeField] private WeaponSystem weaponSystem;
    [SerializeField] private CoreAnimComponent coreAnimComponent;
    [SerializeField] private RecoilAnimation recoilAnimation;
    
    
    //GETTERS & SETTERS//
    public bool ShouldSuicide => shouldSuicide;
    
    //////////////////////////////////////////

    private void Awake()
    {
        blendingLayer = GetComponent<BlendingLayer>();
        weaponSystem = GetComponent<WeaponSystem>();
        coreAnimComponent = GetComponent<CoreAnimComponent>();
        recoilAnimation = GetComponent<RecoilAnimation>();
    }

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

    public void ActivateEnemy()
    {
        blendingLayer.enabled = true;
        weaponSystem.enabled = true;
        coreAnimComponent.enabled = true;
        recoilAnimation.enabled = true;
    }

    public void DesactivateEnemy()
    {
        blendingLayer.enabled = false;
        weaponSystem.enabled = false;
        coreAnimComponent.enabled = false;
        recoilAnimation.enabled = false;
        enabled = false;
    }
}
