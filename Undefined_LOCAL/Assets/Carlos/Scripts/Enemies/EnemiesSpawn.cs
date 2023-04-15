using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemiesSpawn : MonoBehaviour
{
    [SerializeField] private List<Transform> enemiesSpawnsList;
    [SerializeField] private GameObject SoldierEnemyPrefab;
    [SerializeField] private float timeLeftToSpawn;

    private void Start()
    {
        foreach (Transform child in transform)
        {
            enemiesSpawnsList.Add(child);
        }
    }

    public void SpawnEnemies()
    {
        StartCoroutine(SpawnEnemies_Coroutine());
    }
    
    private IEnumerator SpawnEnemies_Coroutine()
    {
        yield return new WaitForSeconds(timeLeftToSpawn);
        
        foreach (Transform enmySpawn in enemiesSpawnsList)
        {
            GameObject enemy = Instantiate(SoldierEnemyPrefab, enmySpawn.position, enmySpawn.rotation);
            Enemy_IA enemyIa = enemy.GetComponent<Enemy_IA>();
            Level1Manager.instance.EnemiesList.Add(enemyIa);
            enemyIa.IsPlayerDetected = true;
        }
    }
}
