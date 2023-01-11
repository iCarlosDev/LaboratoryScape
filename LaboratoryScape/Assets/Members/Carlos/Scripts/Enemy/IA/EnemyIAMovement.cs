using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyIAMovement : MonoBehaviour
{
    //Variables
    [SerializeField] private EnemyScriptsStorage _enemyScriptsStorage;
    
    [SerializeField] private List<Transform> points;
    [SerializeField] private int currentPoint;
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private bool returnable;

    [SerializeField] private bool playerDetected;

    [Header("--- ROTATE ENEMY ---")] 
    [Space(10)] 
    [SerializeField] private float damping;
    [SerializeField] private Vector3 lookPos;
    [SerializeField] private Quaternion rotation;

    [Header("--- LOOK PLAYER ---")] 
    [Space(10)]
    [SerializeField] private LayerMask playerCollider;
    [SerializeField] private Transform searchPlayer;
    [SerializeField] private float sensitivity;
    [SerializeField] private bool lookingPlayer;
    
    [Header("--- SHOOT PLAYER ---")] 
    [Space(10)]
    [SerializeField] private bool shouldShoot;
    [SerializeField] private bool firstTimeShooting;

    //GETTERS && SETTERS//
    public NavMeshAgent NavMeshAgent => _navMeshAgent;
    public List<Transform> Points => points;
    public bool FirstTimeShooting
    {
        get => firstTimeShooting;
        set => firstTimeShooting = value;
    }

    ////////////////////////////////////////////////////

    private void Awake()
    {
        _enemyScriptsStorage = GetComponent<EnemyScriptsStorage>();
        _navMeshAgent = GetComponent<NavMeshAgent>();

        searchPlayer = _enemyScriptsStorage.Weapon.transform.GetChild(6);

        foreach (Transform pointsInArray in transform.parent.GetChild(1))
        {
            points.Add(pointsInArray);
        }
    }

    private void Update()
    {
        _enemyScriptsStorage.FPSController.UpdateFiring();
        
        if (!playerDetected)
        {
            //Si no existen puntos de patrullaje en el enemigo no hará esta lógica (esto puede servir si queremos que hayan enemigos vigilando puntos en concreto sin moverse);
            if (points.Any() && !_enemyScriptsStorage.FieldOfView.canSeePlayer)
            {
                ControlPoints();
            }
            else if (_enemyScriptsStorage.FieldOfView.canSeePlayer)
            {
                PlayerDetected();
                
                if (_enemyScriptsStorage.EnemyIaMovement.Points.Any())
                {
                    _enemyScriptsStorage.FPSController.MoveX1 = 0f;
                    _enemyScriptsStorage.FPSController.MoveY1 = 0f;
                }

                _enemyScriptsStorage.FPSController.ShouldAttack = true;
            }   
        }
        else
        {
            //_enemyScriptsStorage.EnemyIaDecisions.EvaluateRulesUpdate();
            lookPos = _enemyScriptsStorage.FieldOfView.playerRef.transform.position - transform.position;
            lookPos.y = 0;
            rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
            
            LookPlayer();

            if (shouldShoot && !firstTimeShooting)
            {
                ShootPlayer();
            }
        }
    }

    private void ShootPlayer()
    {
        _enemyScriptsStorage.FPSController.OnFirePressed();
        firstTimeShooting = true;
    }

    private void LookPlayer()
    {
        searchPlayer.LookAt(_enemyScriptsStorage.FieldOfView.playerRef.transform.position);

        if (!lookingPlayer)
        {
            shouldShoot = false;
            _enemyScriptsStorage.FPSController.OnFireReleased();
            
            if (searchPlayer.forward.y < _enemyScriptsStorage.Weapon.Muzzle.transform.forward.y)
            {
                _enemyScriptsStorage.LookLayer.AimUp += Time.deltaTime * sensitivity;
            }
            else
            {
                _enemyScriptsStorage.LookLayer.AimUp -= Time.deltaTime * sensitivity;
            }
        }
        else
        {
            shouldShoot = true;
        }

        RaycastHit hit = new RaycastHit();
        Ray ray = new Ray(_enemyScriptsStorage.Weapon.Muzzle.transform.position, _enemyScriptsStorage.Weapon.Muzzle.transform.forward);

        if (Physics.Raycast(ray, out hit, 100f, playerCollider, QueryTriggerInteraction.Ignore))
        {
            lookingPlayer = true;
            
            if (hit.collider.CompareTag("EnemyColliders"))
            {
                shouldShoot = false;
                _enemyScriptsStorage.FPSController.OnFireReleased();
            }
        }
        else
        {
            lookingPlayer = false;
        }
    }

    private void PlayerDetected()
    {
        _navMeshAgent.ResetPath();
        _enemyScriptsStorage.Animator.SetBool("IsMoving", false);
        _enemyScriptsStorage.FieldOfView.enabled = false;
        playerDetected = true;
    }

    /// <summary>
    /// Método para cambiar entre los puntos que tiene que recorrer el enemigo y cuando llegue al último punto haga la misma ruta hacia atrás;
    /// </summary>
    private void ControlPoints()
    {
        //Modificamos el destino del enemigo segun el indice de la lista de puntos a recorrer;
        _navMeshAgent.SetDestination(points[currentPoint].position);
            
        _enemyScriptsStorage.Animator.SetBool("IsMoving", true);
        
        if (Vector3.Distance(transform.position, points[currentPoint].position) < 0.4f)
        {
            if (currentPoint.Equals(points.Count - 1))
            {
                if (returnable)
                {
                    points.Reverse();
                    currentPoint = 0;
                }
                else
                {
                    currentPoint = 0;   
                }
                
                return;
            }

            currentPoint++;
        }
    }
}
