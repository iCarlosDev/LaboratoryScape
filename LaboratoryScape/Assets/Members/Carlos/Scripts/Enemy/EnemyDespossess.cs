using System.Linq;
using Kinemation.FPSFramework.Runtime.Core;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class EnemyDespossess : MonoBehaviour
{
    //Variables
    [SerializeField] private EnemyScriptsStorage _enemyScriptsStorage;
    private CharAnimData _charAnimData;
    private CoreAnimComponent coreAnimComponent;
    
    [Header("--- DESPOSSESS ---")] 
    [SerializeField] private float timeRemaining;
    [SerializeField] private bool shouldSuicide;
    [SerializeField] private bool isPossessed;

    [SerializeField] private Volume _volume;

    //GETTERS & SETTERS//
    public bool ShouldSuicide => shouldSuicide;
    public bool IsPossessed
    {
        get => isPossessed;
        set => isPossessed = value;
    }
    public Volume volume => _volume;

    //////////////////////////////////////////

    private void Awake()
    {
        _enemyScriptsStorage = GetComponent<EnemyScriptsStorage>();
        coreAnimComponent = GetComponent<CoreAnimComponent>();
    }

    private void Update()
    {
        coreAnimComponent.SetCharData(_charAnimData);
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

        DepthOfField dof;
        _volume.profile.TryGet(out dof);
        ChromaticAberration ca;
        _volume.profile.TryGet(out ca);
        dof.nearMaxBlur += Time.deltaTime / 4;
        dof.farMaxBlur += Time.deltaTime / 4;

        if (dof.nearMaxBlur >= 8f)
        {
            ca.intensity.value += Time.deltaTime / 2;
        }

        //Si presionamos "F" o la cuenta atrás llega a 0 el enemigo poseido morirá;
        if (Input.GetKeyDown(KeyCode.F) || timeRemaining <= 0)
        {
            shouldSuicide = true;

            dof.nearMaxBlur = 0f;
            dof.focusDistance.value = 10f;
            ca.intensity.value = 0f;
            volume.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Método para activar al enemigo cuando sea necesario;
    /// </summary>
    public void ActivateEnemyControl()
    {
        CarlosSceneManager.instance.HealthCanvas.SetActive(true);
        CarlosSceneManager.instance.AmmoCanvas.SetActive(true);
        _enemyScriptsStorage.EnemyHealth.HealthTMP = CarlosSceneManager.instance.HealthCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _enemyScriptsStorage.Weapon.AmmoTMP = CarlosSceneManager.instance.AmmoCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        _enemyScriptsStorage.LookLayer.PelvisOffset = new Vector3(0f, -0.04f, 0f);
        _enemyScriptsStorage.LookLayer.AimUp = 0f;
        
        _enemyScriptsStorage.EnemyIaMovement.enabled = false; 
        _enemyScriptsStorage.EnemyIaMovement.NavMeshAgent.enabled = false; 
        _enemyScriptsStorage.FieldOfView.enabled = false;
        _enemyScriptsStorage.EnemyIaDecisions.enabled = false;
        _enemyScriptsStorage.FPSController.IsIa = false;
        
        _enemyScriptsStorage.EnemyHealth.HealthTMP.text = $"{_enemyScriptsStorage.EnemyHealth.CurrentHealth}";
        _enemyScriptsStorage.Weapon.AmmoTMP.text = $"{_enemyScriptsStorage.Weapon.CurrentAmmo} / {_enemyScriptsStorage.Weapon.MaxAmmo}";

        foreach (Rigidbody rigidbodies in _enemyScriptsStorage.EnemyComponentsGetter.Rigidbody)
        {
            rigidbodies.isKinematic = true;
        }
        
        _enemyScriptsStorage.FPSController.ChangePose();
        _enemyScriptsStorage.FPSController.OnFireReleased();
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

        if (_enemyScriptsStorage.EnemyHealth.HealthTMP != null)
        {
            CarlosSceneManager.instance.HealthCanvas.SetActive(false);   
        }

        if (_enemyScriptsStorage.Weapon.AmmoTMP != null)
        {
            CarlosSceneManager.instance.AmmoCanvas.SetActive(false);   
        }

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
