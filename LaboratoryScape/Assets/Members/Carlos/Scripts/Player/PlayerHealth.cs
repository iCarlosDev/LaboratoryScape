using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    //Variables
    [Header("--- HEALTH VALUES ---")]
    [Space(10)]
    [SerializeField] private int currentHealth;
    [SerializeField] private int maxHealth;
    [SerializeField] private bool alreadyDead;

    private void Start()
    {
        maxHealth = 100;
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (!alreadyDead)
        {
            Die();
        }
    }

    private void Die()
    {
        if (currentHealth <= 0)
        {
            alreadyDead = true;
            SceneManager.LoadScene(0);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }
}
