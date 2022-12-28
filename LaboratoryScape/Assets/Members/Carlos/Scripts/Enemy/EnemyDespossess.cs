using System.Linq;
using Kinemation.FPSFramework.Runtime.Core;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyDespossess : MonoBehaviour
{
    //Variables
    [SerializeField] private EnemyScriptsStorage _enemyScriptsStorage;
    
    [Header("--- DESPOSSESS ---")] 
    [SerializeField] private float timeRemaining;
    [SerializeField] private bool shouldSuicide;
    [SerializeField] private bool isPossessed;

    //GETTERS & SETTERS//
    public bool ShouldSuicide => shouldSuicide;
    public bool IsPossessed
    {
        get => isPossessed;
        set => isPossessed = value;
    }

    //////////////////////////////////////////

    private void Awake()
    {
        _enemyScriptsStorage = GetComponent<EnemyScriptsStorage>();
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
    public void ActivateEnemyControl()
    {
        _enemyScriptsStorage.LookLayer.PelvisOffset = new Vector3(0f, -0.04f, 0f);
        _enemyScriptsStorage.LookLayer.AimUp = 0f;
        
        _enemyScriptsStorage.EnemyIaMovement.enabled = false; 
        _enemyScriptsStorage.EnemyIaMovement.NavMeshAgent.enabled = false; 
        _enemyScriptsStorage.FieldOfView.enabled = false;
        _enemyScriptsStorage.EnemyIaDecisions.enabled = false;
        _enemyScriptsStorage.FPSController.IsIa = false;

        foreach (Rigidbody rigidbodies in _enemyScriptsStorage.EnemyComponentsGetter.Rigidbody)
        {
            rigidbodies.isKinematic = true;
        }
    }

    /// <summary>
    /// Método para desactivar al enemigo cuando sea necesario;
    /// </summary>
    public void DesactivateEnemyControl()
    {
        if (_enemyScriptsStorage.EnemyIaMovement.Points.Any())
        {
            _enemyScriptsStorage.FPSController.MoveX1 = 0f;
            _enemyScriptsStorage.FPSController.MoveY1 = 1f;
        }
        
        _enemyScriptsStorage.LookLayer.PelvisOffset = new Vector3(0f, 0.04f, 0f);
        _enemyScriptsStorage.FPSController.enabled = true;
        _enemyScriptsStorage.FieldOfView.enabled = true;

        enabled = false;
    }

    /// <summary>
    /// Método para desactivar los componentes necesarios del enemigo que muere (para que muera de la forma que se desea);
    /// </summary>
    public void EnemyDie()
    {
        if (isPossessed)
        {
            CarlosSceneManager.instance.DespossessParameters();
        }

        _enemyScriptsStorage.CharacterController.enabled = false;
        _enemyScriptsStorage.BlendingLayer.enabled = false;
        _enemyScriptsStorage.CoreAnimComponent.enabled = false;
        _enemyScriptsStorage.RecoilAnimation.enabled = false;
        _enemyScriptsStorage.EnemyIaMovement.enabled = false;
        _enemyScriptsStorage.EnemyIaMovement.NavMeshAgent.enabled = false;
        _enemyScriptsStorage.FieldOfView.enabled = false;
        _enemyScriptsStorage.EnemyIaDecisions.enabled = false;

        foreach (Rigidbody rigidbodies in _enemyScriptsStorage.EnemyComponentsGetter.Rigidbody)
        {
            rigidbodies.isKinematic = false;
        }

        _enemyScriptsStorage.Animator.enabled = false;
        enabled = false;

        GetComponent<CapsuleCollider>().enabled = false;
        
        _enemyScriptsStorage.FPSController.enabled = false;
        CarlosSceneManager.instance.EnmiesList.Remove(_enemyScriptsStorage.FPSController);
        CarlosSceneManager.instance.EnemiesInRangeList.Remove(_enemyScriptsStorage.FPSController);

        _enemyScriptsStorage.FPSController.Weapons[_enemyScriptsStorage.FPSController.Index].transform.parent = null;

        isPossessed = false;
    }
}
