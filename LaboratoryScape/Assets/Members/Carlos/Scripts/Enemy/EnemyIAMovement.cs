using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyIAMovement : MonoBehaviour
{
    //Variables
    [SerializeField] private List<Transform> points;
    [SerializeField] private int currentPoint;
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private Animator _animator;

    public NavMeshAgent NavMeshAgent => _navMeshAgent;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        foreach (Transform pointsInArray in transform.parent.GetChild(1))
        {
            points.Add(pointsInArray);
        }
    }

    private void Update()
    {
        //Si no existen puntos de patrullaje en el enemigo no hará esta lógica (esto puede servir si queremos que hayan enemigos vigilando puntos en concreto sin moverse);
        if (points.Any())
        {
            //Modificamos el destino del enemigo segun el indice de la lista de puntos a recorrer;
            _navMeshAgent.SetDestination(points[currentPoint].position);
            
            _animator.SetBool("IsMoving", true);
            
            ControlPoints();
        }
        else
        {
            _animator.SetBool("IsMoving", false);
        }
    }

    /// <summary>
    /// Método para cambiar entre los puntos que tiene que recorrer el enemigo y cuando llegue al último punto que haga la misma ruta hacia atrás;
    /// </summary>
    private void ControlPoints()
    {
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
