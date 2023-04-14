using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private PlayerScriptStorage _playerScriptStorage;
    
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;
    [SerializeField] private int requiredHealth;

    //GETTERS && SETTERS//
    public int CurrentHealth => currentHealth;
    public int RequiredHealth => requiredHealth;

    ////////////////////////////////
    
    private void Awake()
    {
        _playerScriptStorage = GetComponent<PlayerScriptStorage>();
    }

    void Start()
    {
        maxHealth = 100;
        currentHealth = maxHealth;
        _playerScriptStorage.Animator.SetFloat("Health", currentHealth/100f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TakeDamage(50);
        }
    }

    //Método para quitarle vida al player;
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        _playerScriptStorage.Animator.SetFloat("Health", currentHealth/100f);
        
        //Si la vida de el player llega a 0...;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    //Método para que el player muera;
    private void Die()
    {
        _playerScriptStorage.PlayerMovement.Animator.SetTrigger("Die");
        _playerScriptStorage.EnemyPossess.enabled = false;

        _playerScriptStorage.PlayerHealth.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("StompCollider"))
        {
            TakeDamage(10);
        }
    }
}
