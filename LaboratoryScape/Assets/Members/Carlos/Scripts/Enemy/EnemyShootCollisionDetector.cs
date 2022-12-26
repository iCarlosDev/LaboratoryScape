using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyShootCollisionDetector : MonoBehaviour
{
    [SerializeField] private EnemyScriptsStorage _enemyScriptsStorage;
    

    public EnemyScriptsStorage EnemyScriptsStorage
    {
        get => _enemyScriptsStorage;
        set => _enemyScriptsStorage = value;
    }

    private void hit()
    {
        if (GetComponent<BoxCollider>() != null)
        {
            _enemyScriptsStorage.EnemyDamage.TakeDamage(_enemyScriptsStorage.EnemyDamage.BodyDamage);   
        }
        else if (GetComponent<CapsuleCollider>() != null)
        {
            _enemyScriptsStorage.EnemyDamage.TakeDamage(_enemyScriptsStorage.EnemyDamage.ExtremitiesDamage);
        }
        else
        {
            _enemyScriptsStorage.EnemyDamage.TakeDamage(_enemyScriptsStorage.EnemyDamage.HeadDamage);
        }
    }
}
