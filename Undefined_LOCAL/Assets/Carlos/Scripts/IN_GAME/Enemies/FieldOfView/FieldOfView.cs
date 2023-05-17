using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private EnemyScriptStorage _enemyScriptStorage;
    
    public float radius;
    [Range(0,360)]
    public float angle;

    public GameObject playerRef;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool canSeePlayer;

    private void Awake()
    {
        _enemyScriptStorage = GetComponentInParent<EnemyScriptStorage>();
    }

    private void Start()
    {
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask, QueryTriggerInteraction.Collide);
        Debug.Log(rangeChecks.Length);
        
        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    if (target.gameObject.layer == 12)
                    {
                        _enemyScriptStorage.EnemyIa.IsPlayerDetected = true;
                        target.gameObject.layer = 0;
                    }
                    else
                    {
                        canSeePlayer = true; 
                    }
                }
                else
                {
                    canSeePlayer = false;
                }
            }
            else
                canSeePlayer = false;
        }
        else if (canSeePlayer)
            canSeePlayer = false;
    }
}
