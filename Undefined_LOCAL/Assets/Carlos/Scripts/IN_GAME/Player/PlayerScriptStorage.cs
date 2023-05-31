using System;
using Cinemachine;
using UnityEngine;

public class PlayerScriptStorage : MonoBehaviour
{

    [Header("--- COMPONENTS ---")] 
    [Space(10)] 
    [SerializeField] private Animator _animator;
    [SerializeField] private CinemachineFreeLook freeLookCamera;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private GameObject _canvas;
    
    [Header("--- SCRIPTS ---")]
    [Space(10)]
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private EnemyPossess _enemyPossess;
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private DoorCard _doorCard;
    
    //GETTERS && SETTERS//
    public PlayerMovement PlayerMovement => _playerMovement;
    public EnemyPossess EnemyPossess => _enemyPossess;
    public PlayerHealth PlayerHealth => _playerHealth;
    public Animator Animator => _animator;
    public CinemachineFreeLook FreeLookCamera => freeLookCamera;
    public CinemachineVirtualCamera VirtualCamera => virtualCamera;
    public DoorCard DoorCard => _doorCard;

    ///////////////////////////////
    
    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _enemyPossess = GetComponentInChildren<EnemyPossess>();
        _playerHealth = GetComponent<PlayerHealth>();
        _animator = GetComponent<Animator>();
        freeLookCamera = GetComponentInChildren<CinemachineFreeLook>();
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        _doorCard = GetComponent<DoorCard>();
    }

    private void Update()
    {
        if (PauseMenuManager.instance.IsPaused)
        {
            if (!_canvas.activeSelf) return;
            _canvas.SetActive(false);
        }
        else
        {
            if (_canvas.activeSelf) return;
            _canvas.SetActive(true);
        }
    }
}
