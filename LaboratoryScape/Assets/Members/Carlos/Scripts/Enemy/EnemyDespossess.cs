using System;
using System.Collections;
using System.Collections.Generic;
using Demo.Scripts;
using Demo.Scripts.Runtime;
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
    [SerializeField] private CoreAnimComponent coreAnimComponent;
    [SerializeField] private RecoilAnimation recoilAnimation;
    [SerializeField] private EnemyComponentsGetter _enemyComponentsGetter;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private FPSController _fpsController;
    [SerializeField] private EnemyIAMovement _enemyIaMovement;
    [SerializeField] private Animator _animator;
    
    
    //GETTERS & SETTERS//
    public bool ShouldSuicide => shouldSuicide;
    
    //////////////////////////////////////////

    private void Awake()
    {
        blendingLayer = GetComponent<BlendingLayer>();
        coreAnimComponent = GetComponent<CoreAnimComponent>();
        recoilAnimation = GetComponent<RecoilAnimation>();
        _enemyComponentsGetter = GetComponent<EnemyComponentsGetter>();
        _characterController = GetComponent<CharacterController>();
        _fpsController = GetComponent<FPSController>();
        _enemyIaMovement = GetComponent<EnemyIAMovement>();
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

    /// <summary>
    /// Método para activar al enemigo cuando sea necesario;
    /// </summary>
    public void ActivateEnemy()
    {
        _characterController.enabled = true;
        blendingLayer.enabled = true;
        coreAnimComponent.enabled = true;
        recoilAnimation.enabled = true;
        _enemyIaMovement.enabled = false;
        _enemyIaMovement.NavMeshAgent.enabled = false;
        _animator.SetLayerWeight(1, 0);
        _animator.SetLayerWeight(2, 1);
        _animator.SetLayerWeight(3, 1);
        _animator.SetLayerWeight(4, 1);

        foreach (Rigidbody rigidbodies in _enemyComponentsGetter.Rigidbody)
        {
            rigidbodies.isKinematic = true;
        }
    }

    /// <summary>
    /// Método para desactivar al enemigo cuando sea necesario;
    /// </summary>
    public void DesactivateEnemy()
    {
        _characterController.enabled = false;
        blendingLayer.enabled = false;
        coreAnimComponent.enabled = false;
        recoilAnimation.enabled = false;
        _animator.SetLayerWeight(1, 1);
        _animator.SetLayerWeight(2, 0);
        _animator.SetLayerWeight(3, 0);
        _animator.SetLayerWeight(4, 0);

        enabled = false;
    }

    /// <summary>
    /// Método para desactivar los componentes necesarios del enemigo que muere (para que muera de la forma que se quiere);
    /// </summary>
    public void EnemyDie()
    {
        _characterController.enabled = false;
        blendingLayer.enabled = false;
        coreAnimComponent.enabled = false;
        recoilAnimation.enabled = false;
        _enemyIaMovement.enabled = false;
        _enemyIaMovement.NavMeshAgent.enabled = false;
        _animator.SetLayerWeight(1, 1);
        _animator.SetLayerWeight(2, 0);
        _animator.SetLayerWeight(3, 0);
        _animator.SetLayerWeight(4, 0);

        foreach (Rigidbody rigidbodies in _enemyComponentsGetter.Rigidbody)
        {
            rigidbodies.isKinematic = false;
        }

        _animator.enabled = false;
        enabled = false;

        GetComponent<CapsuleCollider>().enabled = false;
        
        _fpsController.enabled = false;
        CarlosSceneManager.instance.EnmiesList.Remove(_fpsController);
        CarlosSceneManager.instance.EnemiesInRangeList.Remove(_fpsController);

        _fpsController.Weapons[_fpsController.Index].transform.parent = null;
    }
}
