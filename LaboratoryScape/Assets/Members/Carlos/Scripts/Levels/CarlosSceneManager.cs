using System;
using System.Collections;
using System.Collections.Generic;
using Demo.Scripts;
using EPOOutline;
using Kinemation.FPSFramework.Runtime.Core;
using Kinemation.FPSFramework.Runtime.Layers;
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
   [SerializeField] private List<FPSController> enmiesList;
   [SerializeField] private List<FPSController> enemiesInRangeList;
   [SerializeField] private FPSController closestEnemy;
   [SerializeField] private EnemyDespossess enemyDespossess;
   [SerializeField] private bool alreadyPossessed;
   
   //GETTERS & SETTERS//
   
   public List<FPSController> EnemiesInRangeList => enemiesInRangeList;

   ///////////////////////////////////////////

   private void Awake()
   {
      instance = this;
      
      playerController = FindObjectOfType<PlayerController>();
      playerPossess = FindObjectOfType<PlayerPossess>();
   }

   private void Start()
   {
      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = false;
      
      FPSController[] enemiesArray = FindObjectsOfType<FPSController>();

      foreach (var enemy in enemiesArray)
      {
         enemyDespossess = enemy.GetComponent<EnemyDespossess>();
         enemy.enabled = false;
         enemyDespossess.DesactivateEnemy();
         enemy.CameraBone.gameObject.SetActive(false);
         
         enmiesList.Add(enemy);
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
      if (!playerPossess.HaveCooldown)
      {
         if (playerPossess.CanPossess)
         {
            GetClosestEnemy(enemiesInRangeList);
         }
      }
   }

   private void PossessParameters()
   {
      if (!alreadyPossessed)
      {
         playerPossess.CanPossess = false;
         playerController.PlayerCamera.gameObject.SetActive(false);
         playerController.gameObject.SetActive(false);

         enemyDespossess = closestEnemy.GetComponent<EnemyDespossess>();
         
         closestEnemy.CameraBone.gameObject.SetActive(true);
         closestEnemy.enabled = true;
         enemyDespossess.enabled = true;
         enemyDespossess.ActivateEnemy();

         alreadyPossessed = true;
      }
      
      playerController.transform.position = closestEnemy.transform.position;
   }

   #endregion

   #region - ENEMY -

   private void Despossess()
   {
      if (enemyDespossess.ShouldSuicide)
      {
         DespossessParameters();
         enmiesList.Remove(closestEnemy);
         enemiesInRangeList.Remove(closestEnemy);
         Destroy(closestEnemy.gameObject);
      }
   }

   private void DespossessParameters()
   {
      playerController.gameObject.SetActive(true);
      playerController.PlayerCamera.gameObject.SetActive(true);
      playerPossess.ImPossessing = false;
      playerPossess.HaveCooldown = true;
      alreadyPossessed = false;
   }

   #endregion
   
   private void GetClosestEnemy(List<FPSController> enemies)
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
         
         potentialTarget.Outlinable.enabled = false;
      }
      
      closestEnemy.Outlinable.enabled = true;
   }
}
