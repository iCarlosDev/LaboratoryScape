using System;
using System.Collections;
using System.Collections.Generic;
using Demo.Scripts.Runtime;
using EPOOutline;
using Kinemation.FPSFramework.Runtime.Core;
using Kinemation.FPSFramework.Runtime.Layers;
using UnityEngine;

public class EnemyScriptsStorage : MonoBehaviour
{
    public static EnemyScriptsStorage instance;
    
    [Header("--- ENEMY SCRIPTS STORAGE ---")]
    [SerializeField] private BlendingLayer blendingLayer;
    [SerializeField] private CoreAnimComponent coreAnimComponent;
    [SerializeField] private RecoilAnimation recoilAnimation;
    [SerializeField] private EnemyComponentsGetter _enemyComponentsGetter;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private FPSController _fpsController;
    [SerializeField] private EnemyIAMovement _enemyIaMovement;
    [SerializeField] private Animator _animator;
    [SerializeField] private FieldOfView _fieldOfView;
    [SerializeField] private EnemyIADecisions _enemyIaDecisions;
    [SerializeField] private EnemyHealth _enemyHealth;
    [SerializeField] private EnemyDespossess _enemyDespossess;
    [SerializeField] private LookLayer _lookLayer;
    [SerializeField] private Weapon _weapon;
    [SerializeField] private WeaponPoseDetector weaponPoseDetector;
    [SerializeField] private Outlinable _outlinable;
    [SerializeField] private AimColliderDetector aimColliderDetector;


    //GETTERS && SETTERS//
    public BlendingLayer BlendingLayer => blendingLayer;
    public CoreAnimComponent CoreAnimComponent => coreAnimComponent;
    public RecoilAnimation RecoilAnimation => recoilAnimation;
    public EnemyComponentsGetter EnemyComponentsGetter => _enemyComponentsGetter;
    public CharacterController CharacterController => _characterController;
    public FPSController FPSController => _fpsController;
    public EnemyIAMovement EnemyIaMovement => _enemyIaMovement;
    public Animator Animator => _animator;
    public FieldOfView FieldOfView => _fieldOfView;
    public EnemyIADecisions EnemyIaDecisions => _enemyIaDecisions;
    public EnemyHealth EnemyHealth => _enemyHealth;
    public EnemyDespossess EnemyDespossess => _enemyDespossess;
    public LookLayer LookLayer => _lookLayer;
    public Weapon Weapon => _weapon;
    public WeaponPoseDetector WeaponPoseDetector => weaponPoseDetector;
    public Outlinable Outlinable => _outlinable;
    public AimColliderDetector AimColliderDetector => aimColliderDetector;
    /////////////////////////////////////////////////////////////////

    private void Awake()
    {
        instance = this;
        
        blendingLayer = GetComponent<BlendingLayer>();
        coreAnimComponent = GetComponent<CoreAnimComponent>();
        recoilAnimation = GetComponent<RecoilAnimation>();
        _enemyComponentsGetter = GetComponent<EnemyComponentsGetter>();
        _characterController = GetComponent<CharacterController>();
        _fpsController = GetComponent<FPSController>();
        _enemyIaMovement = GetComponent<EnemyIAMovement>();
        _animator = GetComponent<Animator>();
        _fieldOfView = GetComponent<FieldOfView>();
        _enemyIaDecisions = GetComponent<EnemyIADecisions>();
        _enemyHealth = GetComponent<EnemyHealth>();
        _enemyDespossess = GetComponent<EnemyDespossess>();
        _lookLayer = GetComponent<LookLayer>();
        _weapon = _fpsController.Weapons[_fpsController.Index];
        weaponPoseDetector = _fpsController.CameraBone.transform.GetChild(0).GetComponent<WeaponPoseDetector>();
        _outlinable = GetComponent<Outlinable>();
        aimColliderDetector = _fpsController.CameraBone.transform.GetChild(1).GetComponent<AimColliderDetector>();
    }
}
