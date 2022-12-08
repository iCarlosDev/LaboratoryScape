using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarlosSceneManager : MonoBehaviour
{
   //Variables
   [Header("--- PLAYER ---")]
   [Space(10)]
   [SerializeField] private PlayerController playerController;
   
   [Header("--- ENEMY ---")]
   [Space(10)]
   [SerializeField] private EnemyController enemyController;
   private void Awake()
   {
      playerController = FindObjectOfType<PlayerController>();
      enemyController = FindObjectOfType<EnemyController>();
   }
}
