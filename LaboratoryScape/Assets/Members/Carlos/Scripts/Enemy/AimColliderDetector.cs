using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimColliderDetector : MonoBehaviour
{
   [SerializeField] private EnemyScriptsStorage _enemyScriptsStorage;
   [SerializeField] private bool isColliding;

   //GETTERS && SETTERS//
   public bool IsColliding
   {
      get => isColliding;
      set => isColliding = value;
   }

   ////////////////////////////////////

   private void Awake()
   {
      _enemyScriptsStorage = GetComponentInParent<EnemyScriptsStorage>();
   }

   private void Start()
   {
      gameObject.SetActive(false);
   }

   private void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag("Obstacle"))
      {
         isColliding = true;
         _enemyScriptsStorage.FPSController.WeaponBlockFlag = true;
      }
   }

   private void OnTriggerExit(Collider other)
   {
      if (other.CompareTag("Obstacle"))
      {
         isColliding = false;
      }
   }
}
