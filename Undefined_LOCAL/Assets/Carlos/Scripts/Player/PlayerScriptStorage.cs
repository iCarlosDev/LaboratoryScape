using UnityEngine;

public class PlayerScriptStorage : MonoBehaviour
{

    [Header("--- COMPONENTS ---")] 
    [Space(10)] 
    [SerializeField] private Animator _animator;
    
    [Header("--- SCRIPTS ---")]
    [Space(10)]
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private EnemyPossess _enemyPossess;
    [SerializeField] private PlayerHealth _playerHealth;
    
    //GETTERS && SETTERS//
    public PlayerMovement PlayerMovement => _playerMovement;
    public EnemyPossess EnemyPossess => _enemyPossess;
    public PlayerHealth PlayerHealth => _playerHealth;
    public Animator Animator => _animator;

    ///////////////////////////////
    
    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _enemyPossess = GetComponent<EnemyPossess>();
        _playerHealth = GetComponent<PlayerHealth>();
        _animator = GetComponent<Animator>();
    }
}
