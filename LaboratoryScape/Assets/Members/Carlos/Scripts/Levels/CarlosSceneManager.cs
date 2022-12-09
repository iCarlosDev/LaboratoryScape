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
   [SerializeField] private List<EnemyController> enmiesList;
   [SerializeField] private EnemyController closestEnemy;

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

      foreach (var enemies in FindObjectsOfType<EnemyController>())
      {
         enemies.enabled = false;
         enemies.GetComponent<EnemyDespossess>().enabled = false;
         enmiesList.Add(enemies);
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
            GetClosestEnemy(enmiesList);
         }
         else
         {
            closestEnemy = null;
            
            foreach (var enemy in enmiesList)
            {
               enemy.gameObject.GetComponent<Outlinable>().enabled = false;
            }
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
         enmiesList.Remove(closestEnemy);
         Destroy(closestEnemy.gameObject);
      }
   }

   private void DespossessParameters()
   {
      playerController.gameObject.SetActive(true);
      playerController.PlayerCamera.gameObject.SetActive(true);
      playerPossess.ImPossessing = false;
      playerPossess.HaveCooldown = true;
   }

   #endregion
   
   private void GetClosestEnemy(List<EnemyController> enemies)
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
