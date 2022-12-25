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

    [SerializeField] private bool playerDetected;

    public NavMeshAgent NavMeshAgent => _navMeshAgent;

    private void Awake()
    {
        _enemyScriptsStorage = GetComponent<EnemyScriptsStorage>();
        _navMeshAgent = GetComponent<NavMeshAgent>();

        foreach (Transform pointsInArray in transform.parent.GetChild(1))
        {
            points.Add(pointsInArray);
        }
    }

    private void Update()
    {
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
            }   
        }
        else
        {
            _enemyScriptsStorage.EnemyIaDecisions.EvaluateRulesUpdate();
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
        
        if (Vector3.Distance(transform.position, points[currentPoint].position) < 0.1f)
        {
            if (currentPoint.Equals(points.Count - 1))
            {
                points.Reverse();
                currentPoint = 0;
                return;
            }
            
            currentPoint++;
        }
    }
}
