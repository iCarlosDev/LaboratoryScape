using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private EnemyScriptsStorage _enemyScriptsStorage;
    
    //Variables
    [Header("--- HEALTH VALUES ---")]
    [Space(10)]
    [SerializeField] private int currentHealth;
    [SerializeField] private int maxHealth;
    [SerializeField] private bool alreadyDead;

    [Header("--- DAMAGE VALUES ---")]
    [Space(10)]
    [SerializeField] private int headDamage;
    [SerializeField] private int bodyDamage;
    [SerializeField] private int extremitiesDamage;

    //GETTERS && SETTERS//
    public int HeadDamage => headDamage;
    public int BodyDamage => bodyDamage;
    public int ExtremitiesDamage => extremitiesDamage;
    
    ////////////////////////////////////////////
    
    private void Awake()
    {
        _enemyScriptsStorage = GetComponent<EnemyScriptsStorage>();
    }

    private void Start()
    {
        maxHealth = 100;
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (!alreadyDead)
        {
            IsDead();   
        }
    }

    private void IsDead()
    {
        if (currentHealth <= 0)
        {
            alreadyDead = true;
            _enemyScriptsStorage.EnemyDespossess.EnemyDie();   
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }
}
