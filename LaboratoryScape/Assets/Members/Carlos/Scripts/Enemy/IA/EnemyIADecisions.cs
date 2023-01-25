using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyIADecisions : MonoBehaviour
{
    [SerializeField] private EnemyScriptsStorage _enemyScriptsStorage;

    [Header("--- COVER POINTS ---")] 
    [Space(10)] 
    [SerializeField] private List<GameObject> coverPoints;

    [Header("--- SHOOT PLAYER ---")] 
    [Space(10)]
    [SerializeField] private bool shouldShoot;
    [SerializeField] private bool firstTimeShooting;
    
    //GETTERS && SETTERS//
    public bool ShouldShoot
    {
        get => shouldShoot;
        set => shouldShoot = value;
    }
    public bool FirstTimeShooting
    {
        get => firstTimeShooting;
        set => firstTimeShooting = value;
    }

    //////////////////////////////

    private void Awake()
    {
        coverPoints.AddRange(GameObject.FindGameObjectsWithTag("CoverPoints"));
    }

    private void Cover()
    {
        
    }

    public void ShootPlayer()
    {
        if (shouldShoot && !firstTimeShooting)
        {
            _enemyScriptsStorage.FPSController.OnFirePressed();
            firstTimeShooting = true;       
        }
    }

    /*public void EvaluateRulesUpdate()
    {
        timeSinceLastEvaluation += Time.deltaTime;
        if (timeSinceLastEvaluation >= evaluationInterval)
        {
            timeSinceLastEvaluation = 0;
            EvaluateRules();
        }
    }*/
}
