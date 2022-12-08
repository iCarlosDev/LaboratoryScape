using System;
using System.Collections;
using System.Collections.Generic;
using EPOOutline;
using UnityEngine;

public class CarlosSceneManager : MonoBehaviour
{
   //Variables
   [Header("--- PLAYER ---")]
   [Space(10)]
   [SerializeField] private PlayerController playerController;
   
   [Header("--- ENEMY ---")]
   [Space(10)]
   [SerializeField] private EnemyController[] enemyController;
   [SerializeField] private Transform closestEnemy;
   private void Awake()
   {
      playerController = FindObjectOfType<PlayerController>();
      enemyController = FindObjectsOfType<EnemyController>();
   }

   private void Update()
   {
      Possession();
   }

   #region - PLAYER -

   private void Possession()
   {
      if (playerController.CanPossess)
      {
         GetClosestEnemy(enemyController);
      }
      else
      {
         foreach (var enemy in enemyController)
         {
            enemy.gameObject.GetComponent<Outlinable>().enabled = false;
         }
      }
   }

   #endregion
   
   private Transform GetClosestEnemy(EnemyController enemies)
   {
      Debug.Log(enemies);
      
      Transform bestTarget = null;
      float closestDistanceSqr = Mathf.Infinity;
      Vector3 currentPos = playerController.transform.position;

      foreach (Transform potentialTarget in enemies.transform)
      {
         Vector3 directionToTarget = potentialTarget.position - currentPos;
         float dSqrToTarget = directionToTarget.sqrMagnitude;

         if (dSqrToTarget < closestDistanceSqr)
         {
            closestDistanceSqr = dSqrToTarget;
            bestTarget = potentialTarget;
         }
      }
      
      bestTarget.parent.gameObject.GetComponent<Outlinable>().enabled = true;

      return bestTarget;
   }
}
