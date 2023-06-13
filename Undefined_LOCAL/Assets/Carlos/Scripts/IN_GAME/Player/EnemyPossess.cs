using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using EPOOutline;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyPossess : MonoBehaviour, I_Interact
{
    [SerializeField] private PlayerScriptStorage _playerScriptStorage;
    
    [Header("--- ENEMY POSSESS ---")]
    [Space(10)]
    [SerializeField] private Enemy_IA closestEnemy;
    [SerializeField] private List<Enemy_IA> enemiesInRangeList;

    [Header("--- POSSESS PARAMETERS ---")] 
    [Space(10)] 
    [SerializeField] private GameObject enemyFP;
    [SerializeField] private float cooldownTime;
    [SerializeField] private bool haveCooldown;
    [SerializeField] private bool canPossess = true;
    
    ////NO SE PUEDE IMPLEMENTAR PORQ SOY RETRASAO////
    public Outlinable outliner { get; set; }

    private void Awake()
    {
        _playerScriptStorage = GetComponentInParent<PlayerScriptStorage>();
        enemyFP = FindObjectOfType<SoldierFP_Controller>()?.gameObject;
    }
    
    private void OnEnable()
    {
        if (haveCooldown)
        {
            StartCoroutine(CooldownCoroutine());
        }
        
        StartCoroutine(GetClosestEnemyRoutine());
        PossessIndicator();
    }

    private void OnDisable()
    {
        haveCooldown = true;
        _playerScriptStorage.MainCamera.SetActive(false);
    }

    void Start()
    {
        enemyFP?.SetActive(false);
        
        StartCoroutine(GetClosestEnemyRoutine());
        PossessIndicator();
    }

    private void PossessIndicator()
    {
        if (_playerScriptStorage.Animator != null)
        {
            if (!haveCooldown)
            {
                _playerScriptStorage.Animator.SetTrigger("PossessIndicatorEnabled");
            }
            else
            {
                _playerScriptStorage.Animator.SetTrigger("PossessIndicatorDisabled");
            } 
        }
    }
    
    private IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(cooldownTime);
        haveCooldown = false;
        PossessIndicator();

        if (enemiesInRangeList.Count != 0)
        {
            SetTextInteract(true);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && canPossess && !haveCooldown && !_playerScriptStorage.PlayerMovement.IsInConduct)
        {
            PossessEnemy();
        }
    }

    //Método para poseer a un enemigo;
    private void PossessEnemy()
    {
        if (closestEnemy == null) return;
      
        AudioManager.instance.Play("PlayerPossess");
        SetTextInteract(false);
        AddEnemyCards();
        enemyFP.transform.position = closestEnemy.transform.position;
        enemyFP.transform.rotation = closestEnemy.transform.rotation;
        enemyFP.SetActive(true);
        enemyFP.GetComponent<EnemyDespossess>().StartUp(closestEnemy, transform.parent.gameObject);
        enemyFP.SendMessage("OnEnable");
        closestEnemy.EnemyScriptStorage.Outlinable.enabled = false;
        closestEnemy.EnemyScriptStorage.EnemyIa.enabled = false;
        closestEnemy.gameObject.SetActive(false);
        closestEnemy = null;
        enemiesInRangeList.Clear();

        canPossess = false;

        //Recorremos todos los enemigos en escena y cambiamos el player de referencia que tienen;
        foreach (Enemy_IA enemy in Level1Manager.instance.EnemiesList)
        {
            //Igualamos el player de referencia al EnemyFP;
            enemy.PlayerRef = enemyFP.transform;
        }

        transform.parent.gameObject.SetActive(false);
    }

    private void AddEnemyCards()
    {
        if (_playerScriptStorage.DoorCard.DoorCardEnumList.Count == 4) return;

        DoorCard enemyFP_DoorCard = enemyFP.GetComponent<DoorCard>();

        foreach (DoorCard.DoorCardStatus EnemydoorCardStatus in closestEnemy.EnemyScriptStorage.DoorCard.DoorCardEnumList)
        {
            if (!_playerScriptStorage.DoorCard.DoorCardEnumList.Contains(EnemydoorCardStatus))
            {
                _playerScriptStorage.DoorCard.DoorCardEnumList.Add(EnemydoorCardStatus);
            }

            if (!enemyFP_DoorCard.DoorCardEnumList.Contains(EnemydoorCardStatus))
            {
                enemyFP_DoorCard.DoorCardEnumList.Add(EnemydoorCardStatus);
            }
        }
    }
    
    #region - CLOSEST ENEMY DETECTION -
    
    //Corrutina para encontrar el enemigo más cercano al Player cada 0.1s;
    private IEnumerator GetClosestEnemyRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);

        while (true)
        {
            yield return wait;

            //Si no tenemos cooldown de posesión y podemos poseer a un enemigo...;
            if (!haveCooldown && canPossess)
            {
                GetClosestEnemy(enemiesInRangeList);
            }
        }
    }
    
    /// <summary>
    /// Método para calcular que enemigo está más cerca del player;
    /// </summary>
    /// <param name="enemies"></param>
    private void GetClosestEnemy(List<Enemy_IA> enemies)
    {
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        //Recorremos todos los enemigos dentro de la lista "EnemiesInRangeList";
        foreach (Enemy_IA potentialTarget in enemies)
        {
            //si el enemigo puede poseerse...;
            if (potentialTarget.CanBePossessed)
            {
                //Guardamos en esta variable la distancia entre el enemigo que estamos recorriendo y el player;
                Vector3 directionToTarget = potentialTarget.transform.position - currentPos;
                //Guardamos en esta variable la magnitud de la distancia entre el enemigo y el player;
                float dSqrToTarget = directionToTarget.sqrMagnitude;

                //Si la distancia entre un enemigo es menor que la más cercana anteriormente...;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    //Esta se convierte en la más cercana;
                    closestDistanceSqr = dSqrToTarget;
                    //El enemigo que se esté recorriendo será el más cercano;
                    closestEnemy = potentialTarget;
                }
            }
         
            potentialTarget.EnemyScriptStorage.Outlinable.enabled = false;
        }

        if (closestEnemy != null && _playerScriptStorage.PlayerHealth.CurrentHealth > 0)
        {
            closestEnemy.EnemyScriptStorage.Outlinable.enabled = true;
        }
    }
    
    #endregion

    #region - ON TRIGGERS -

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            //Recorremos todos los enemigos que entran en nuestro collider;
            foreach (Enemy_IA enemy in enemiesInRangeList)
            {
                //Comprobamos que el enemigo que entre no sea el mismo que ya existe en la lista, para no duplicar enemigos;
                if (enemy.transform.parent.name.Equals(other.transform.parent.name))
                {
                    return;
                }
            }

            //Si el enemigo no puede ser poseido no se hará la lógica restante;
            if (!other.GetComponent<Enemy_IA>().CanBePossessed)
            {
               return; 
            }
            
            enemiesInRangeList.Add(other.GetComponent<Enemy_IA>());
            canPossess = true;

            if (!haveCooldown)
            {
                SetTextInteract(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyScriptStorage enemy = other.GetComponent<EnemyScriptStorage>();
            
            enemy.Outlinable.enabled = false;
            enemiesInRangeList.Remove(enemy.EnemyIa);
            
            //Cuando la lista esté vacía...;
            if (enemiesInRangeList.Count == 0)
            {
                canPossess = false;
                closestEnemy = null;
            }
            
            SetTextInteract(false);
        }
    }
    
    #endregion

    
    public void SetOultine(bool shouldActivate)
    {
        //no se puede implementar porq soy retrasao
    }

    public void SetTextInteract(bool shouldShow)
    {
        Level1Manager.instance.InteractCanvas.SetActive(shouldShow);
        Level1Manager.instance.InteractCanvas.GetComponentInChildren<TextMeshProUGUI>().text = $"Press F to Possess";
    }
}
