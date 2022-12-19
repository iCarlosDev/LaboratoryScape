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
    [SerializeField] private EnemyComponentsGetter _enemyComponentsGetter;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Animator _animator;
    
    
    //GETTERS & SETTERS//
    public bool ShouldSuicide => shouldSuicide;
    
    //////////////////////////////////////////

    private void Awake()
    {
        blendingLayer = GetComponent<BlendingLayer>();
        weaponSystem = GetComponent<WeaponSystem>();
        coreAnimComponent = GetComponent<CoreAnimComponent>();
        recoilAnimation = GetComponent<RecoilAnimation>();
        _enemyComponentsGetter = GetComponent<EnemyComponentsGetter>();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
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
        //Nada más poseamos a un enemigo empezará una cuenta atrás;
        timeRemaining -= Time.deltaTime;
        
        //Si presionamos "F" o la cuenta atrás llega a 0 el enemigo poseido morirá;
        if (Input.GetKeyDown(KeyCode.F) || timeRemaining <= 0)
        {
            shouldSuicide = true;
        }
    }

    //Método para activar al enemigo cuando sea necesario;
    public void ActivateEnemy()
    {
        _characterController.enabled = true;
        blendingLayer.enabled = true;
        weaponSystem.enabled = true;
        coreAnimComponent.enabled = true;
        recoilAnimation.enabled = true;

        foreach (Rigidbody rigidbodies in _enemyComponentsGetter.Rigidbody)
        {
            rigidbodies.isKinematic = true;
        }
    }

    //Método para desactivar al enemigo cuando sea necesario;
    public void DesactivateEnemy()
    {
        _characterController.enabled = false;
        blendingLayer.enabled = false;
        weaponSystem.enabled = false;
        coreAnimComponent.enabled = false;
        recoilAnimation.enabled = false;

        enabled = false;
    }

    public void EnemyDie()
    {
        _characterController.enabled = false;
        blendingLayer.enabled = false;
        weaponSystem.enabled = false;
        coreAnimComponent.enabled = false;
        recoilAnimation.enabled = false;

        foreach (Rigidbody rigidbodies in _enemyComponentsGetter.Rigidbody)
        {
            rigidbodies.isKinematic = false;
        }

        _animator.enabled = false;
        enabled = false;
    }
}
