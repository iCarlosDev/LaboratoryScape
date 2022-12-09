using System;
using System.Collections;
using System.Collections.Generic;
using EPOOutline;
using UnityEngine;

public class CarlosSceneManager : MonoBehaviour
{
   //Variables
   public static CarlosSceneManager instance;
   
   [Header("--- PLAYER ---")]
   [Space(10)]
   [SerializeField] private PlayerController playerController;
   [SerializeField] private PlayerPossess playerPossess;
   
   [Header("--- ENEMY ---")]
   [Space(10)]
   [SerializeField] private EnemyController[] enemiesController;
   [SerializeField] private EnemyDespossess[] enemiesDespossess;
   [SerializeField] private EnemyController closestEnemy;

   private void Awake()
   {
      instance = this;
      
      playerController = FindObjectOfType<PlayerController>();
      playerPossess = FindObjectOfType<PlayerPossess>();
      enemiesController = FindObjectsOfType<EnemyController>();
      enemiesDespossess = FindObjectsOfType<EnemyDespossess>();
   }

   private void Start()
   {
      foreach (var enemies in enemiesController)
      {
         enemies.enabled = false;
      }

      foreach (var enemiesDespossess in enemiesDespossess)
      {
         enemiesDespossess.enabled = false;
      }
   }

   private void Update()
   {
      MarkPossession();

      if (playerPossess.ImPossessing)
      {
         PossessParameters();
         Despossess();
      }
   }

   #region - PLAYER -

   private void MarkPossession()
   {
      if (playerPossess.CanPossess)
      {
         GetClosestEnemy(enemiesController);
      }
      else
      {
         foreach (var enemy in enemiesController)
         {
            enemy.gameObject.GetComponent<Outlinable>().enabled = false;
         }
      }
   }

   private void PossessParameters()
   {
      playerPossess.CanPossess = false;
      playerController.PlayerCamera.gameObject.SetActive(false);
      playerController.gameObject.SetActive(false);
      playerController.transform.position = closestEnemy.transform.position;
      
      closestEnemy.EnemyCamera.gameObject.SetActive(true);
      closestEnemy.enabled = true;
      closestEnemy.GetComponent<EnemyDespossess>().enabled = true;
   }

   #endregion

   #region - ENEMY -

   private void Despossess()
   {
      if (closestEnemy.GetComponent<EnemyDespossess>().ShouldSuicide)
      {
         DespossessParameters();
         closestEnemy.gameObject.SetActive(false);
      }
   }

   private void DespossessParameters()
   {
      playerController.gameObject.SetActive(true);
      playerController.PlayerCamera.gameObject.SetActive(true);
      playerPossess.ImPossessing = false;
   }

   #endregion
   
   private void GetClosestEnemy(EnemyController[] enemies)
   {
      float closestDistanceSqr = Mathf.Infinity;
      Vector3 currentPos = playerController.transform.position;

      foreach (var potentialTarget in enemies)
      {
         if (potentialTarget.CanBePossess)
         {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPos;
            float dSqrToTarget = directionToTarget.sqrMagnitude;

            if (dSqrToTarget < closestDistanceSqr)
            {
               closestDistanceSqr = dSqrToTarget;
               closestEnemy = potentialTarget;
            }
         }
         
         potentialTarget.GetComponent<Outlinable>().enabled = false;
      }
      
      closestEnemy.GetComponent<Outlinable>().enabled = true;
   }
}
