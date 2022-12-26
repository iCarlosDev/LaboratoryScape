using UnityEngine;

public class EnemyDespossess : MonoBehaviour
{
    //Variables
    [SerializeField] private EnemyScriptsStorage _enemyScriptsStorage;
    
    [Header("--- DESPOSSESS ---")] 
    [SerializeField] private float timeRemaining;
    [SerializeField] private bool shouldSuicide;

    //GETTERS & SETTERS//
    public bool ShouldSuicide => shouldSuicide;

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
    public void ActivateEnemy()
    {
        _enemyScriptsStorage.CharacterController.enabled = true;
       _enemyScriptsStorage.BlendingLayer.enabled = true;
       _enemyScriptsStorage.CoreAnimComponent.enabled = true;
       _enemyScriptsStorage.RecoilAnimation.enabled = true;
       _enemyScriptsStorage.EnemyIaMovement.enabled = false;
       _enemyScriptsStorage.EnemyIaMovement.NavMeshAgent.enabled = false;
       _enemyScriptsStorage.FieldOfView.enabled = false;
       _enemyScriptsStorage.EnemyIaDecisions.enabled = false;
       _enemyScriptsStorage.Animator.SetLayerWeight(1, 0);
       _enemyScriptsStorage.Animator.SetLayerWeight(2, 1);
       _enemyScriptsStorage.Animator.SetLayerWeight(3, 1);
       _enemyScriptsStorage.Animator.SetLayerWeight(4, 1);

        foreach (Rigidbody rigidbodies in _enemyScriptsStorage.EnemyComponentsGetter.Rigidbody)
        {
            rigidbodies.isKinematic = true;
        }
    }

    /// <summary>
    /// Método para desactivar al enemigo cuando sea necesario;
    /// </summary>
    public void DesactivateEnemy()
    {
        _enemyScriptsStorage.CharacterController.enabled = false;
        _enemyScriptsStorage.BlendingLayer.enabled = false;
        _enemyScriptsStorage.CoreAnimComponent.enabled = false;
        _enemyScriptsStorage.RecoilAnimation.enabled = false;
        _enemyScriptsStorage.FieldOfView.enabled = true;
        _enemyScriptsStorage.Animator.SetLayerWeight(1, 1);
        _enemyScriptsStorage.Animator.SetLayerWeight(2, 0);
        _enemyScriptsStorage.Animator.SetLayerWeight(3, 0);
        _enemyScriptsStorage.Animator.SetLayerWeight(4, 0);

        enabled = false;
    }

    /// <summary>
    /// Método para desactivar los componentes necesarios del enemigo que muere (para que muera de la forma que se desea);
    /// </summary>
    public void EnemyDie()
    {
        _enemyScriptsStorage.CharacterController.enabled = false;
        _enemyScriptsStorage.BlendingLayer.enabled = false;
        _enemyScriptsStorage.CoreAnimComponent.enabled = false;
        _enemyScriptsStorage.RecoilAnimation.enabled = false;
        _enemyScriptsStorage.EnemyIaMovement.enabled = false;
        _enemyScriptsStorage.EnemyIaMovement.NavMeshAgent.enabled = false;
        _enemyScriptsStorage.FieldOfView.enabled = false;
        _enemyScriptsStorage.EnemyIaDecisions.enabled = false;
        _enemyScriptsStorage.Animator.SetLayerWeight(1, 1);
        _enemyScriptsStorage.Animator.SetLayerWeight(2, 0);
        _enemyScriptsStorage.Animator.SetLayerWeight(3, 0);
        _enemyScriptsStorage.Animator.SetLayerWeight(4, 0);

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
    }
}
