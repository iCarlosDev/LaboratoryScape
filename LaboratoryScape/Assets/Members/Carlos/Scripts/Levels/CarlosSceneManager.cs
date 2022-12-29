using System;
using System.Collections;
using System.Collections.Generic;
using Demo.Scripts;
using Demo.Scripts.Runtime;
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
   [SerializeField] private GameObject health_Canvas;
   [SerializeField] private bool alreadyPossessed;
   
   //GETTERS & SETTERS//
   public List<FPSController> EnemiesInRangeList => enemiesInRangeList;
   public List<FPSController> EnmiesList => enmiesList;
   public GameObject HealthCanvas
   {
      get => health_Canvas;
      set => health_Canvas = value;
   }

   ///////////////////////////////////////////

   private void Awake()
   {
      instance = this;
      
      playerController = FindObjectOfType<PlayerController>();
      playerPossess = playerController.GetComponent<PlayerPossess>();
      health_Canvas = GameObject.Find("EnemyHealth_Canvas");
   }

   private void Start()
   {
      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = false;

      PlayerScriptsStorage.instance.PlayerHealth.HealthCanvas.SetActive(true);
      health_Canvas.SetActive(false);

      FPSController[] enemiesArray = FindObjectsOfType<FPSController>();

      //Recorremos cada enemigo para desactivarlo al empezar y añadimos a la lista "enmiesList" todos los enemigos de la escena;
      foreach (var enemy in enemiesArray)
      {
         enemyDespossess = enemy.GetComponent<EnemyDespossess>();
         enemy.enabled = false;
         enemyDespossess.DesactivateEnemyControl();
         enemy.CameraBone.gameObject.SetActive(false);
         enemy.IsIa = true;

         enmiesList.Add(enemy);
      }

      StartCoroutine(GetClosestEnemyRoutine());
   }

   private void Update()
   {
      //Si estamos poseiendo un enemigo...
      if (playerPossess.ImPossessing)
      {
         PossessParameters();
         Despossess();
      }
   }

   #region - PLAYER -

   /// <summary>
   /// Método para controlar que sucede cuando posees a un enemigo;
   /// </summary>
   private void PossessParameters()
   {
      //Comprobamos si hemos poseido, desactivamos al player y activamos al enemigo poseido;
      if (!alreadyPossessed)
      {
         playerPossess.CanPossess = false;
         PlayerScriptsStorage.instance.PlayerHealth.HealthCanvas.SetActive(false);
         playerController.PlayerCamera.gameObject.SetActive(false);
         playerController.gameObject.SetActive(false);

         enemyDespossess = closestEnemy.GetComponent<EnemyDespossess>();

         enemyDespossess.IsPossessed = true;
         closestEnemy.CameraBone.gameObject.SetActive(true);
         closestEnemy.enabled = true;
         enemyDespossess.enabled = true;
         enemyDespossess.ActivateEnemyControl();

         alreadyPossessed = true;
      }
      
      //Actualizamos la posición del player(desactivado) para cuando desposeamos al enemigo aparecer en la misma posición donde ha sido desposeido;
      playerController.transform.position = new Vector3(closestEnemy.transform.position.x, closestEnemy.transform.position.y + 1f, closestEnemy.transform.position.z);
      closestEnemy.MoveX1 = Input.GetAxis("Horizontal");
      closestEnemy.MoveY1 = Input.GetAxis("Vertical");
   }

   #endregion

   #region - ENEMY -

   /// <summary>
   /// Método donde se modifican parametros al poseer un enemigo;
   /// </summary>
   private void Despossess()
   {
      //Si el enemigo debe suicidarse... lo desposeeremos, lo eliminaremos del la lista de enemigos y morirá;
      if (enemyDespossess.ShouldSuicide)
      {
         DespossessParameters();
         enmiesList.Remove(closestEnemy);
         enemiesInRangeList.Remove(closestEnemy);
         enemyDespossess.EnemyDie();
         closestEnemy.enabled = false;
         closestEnemy = null;
      }
   }

   /// <summary>
   /// Método donde se modifican parametros al desposeer un enemigo;
   /// </summary>
   public void DespossessParameters()
   {
      closestEnemy.CameraBone.gameObject.SetActive(false);
      //closestEnemy.Outlinable.enabled = false;
      
      playerController.gameObject.SetActive(true);
      playerController.PlayerCamera.gameObject.SetActive(true);
      PlayerScriptsStorage.instance.PlayerHealth.HealthCanvas.SetActive(true);
      playerPossess.ImPossessing = false;
      playerPossess.HaveCooldown = true;
      alreadyPossessed = false;
   }

   #endregion

   /// <summary>
   /// Método para buscar al enemigo más cercano siempre y cuando no tengamos cooldown de posesión y esté dentro del rango de posesión del player;
   /// </summary>
   private IEnumerator GetClosestEnemyRoutine()
   {
      WaitForSeconds wait = new WaitForSeconds(0.1f);

      while (true)
      {
         yield return wait;

         if (!playerPossess.HaveCooldown && playerPossess.CanPossess)
         {
            GetClosestEnemy(EnemiesInRangeList);  
         }
      }
   }
   
   /// <summary>
   /// Método para calcular que enemigo está más cerca del player;
   /// </summary>
   /// <param name="enemies"></param>
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
         
         //potentialTarget.Outlinable.enabled = false;
      }
      
      //closestEnemy.Outlinable.enabled = true;
   }
}
