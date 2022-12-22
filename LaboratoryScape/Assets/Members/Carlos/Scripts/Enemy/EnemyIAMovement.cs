using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyIAMovement : MonoBehaviour
{
    //Variables
    [SerializeField] private List<GameObject> points;
    [SerializeField] private int currentPoint;
    [SerializeField] private NavMeshAgent _navMeshAgent;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (points.Any())
        {
            _navMeshAgent.SetDestination(points[currentPoint].transform.position);
        
            ControlPoints();
        }
    }

    private void ControlPoints()
    {
        if (Vector3.Distance(transform.position, points[currentPoint].transform.position) < 0.1f)
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
