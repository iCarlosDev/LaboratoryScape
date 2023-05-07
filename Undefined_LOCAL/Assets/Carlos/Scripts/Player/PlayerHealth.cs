using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private PlayerScriptStorage _playerScriptStorage;
    
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;
    [SerializeField] private int requiredHealth;

    [SerializeField] private Slider healthSlider;
    
    //GETTERS && SETTERS//
    public int CurrentHealth => currentHealth;
    public int RequiredHealth => requiredHealth;

    ////////////////////////////////
    
    private void Awake()
    {
        _playerScriptStorage = GetComponent<PlayerScriptStorage>();
        healthSlider = GetComponentInChildren<Slider>();
    }

    void Start()
    {
        maxHealth = 100;
        currentHealth = maxHealth;
        healthSlider.value = currentHealth;
        _playerScriptStorage.Animator.SetFloat("Health", currentHealth/100f);
    }

    private void OnEnable()
    {
        _playerScriptStorage.Animator.SetFloat("Health", currentHealth/100f);
    }

    //Método para quitarle vida al player;
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthSlider.value = currentHealth;
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
        _playerScriptStorage.Animator.SetTrigger("Die");
        _playerScriptStorage.EnemyPossess.enabled = false;

        _playerScriptStorage.PlayerHealth.enabled = false;
        
        StartCoroutine(RestartGame_Coroutine());
    }

    private IEnumerator RestartGame_Coroutine()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("StompCollider"))
        {
            TakeDamage(10);
        }
    }
}
