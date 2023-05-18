using System;
using System.Collections;
using IE.RichFX;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private PlayerScriptStorage _playerScriptStorage;
    
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;
    [SerializeField] private int requiredHealth;

    [SerializeField] private Slider healthSlider;

    [Header("--- HEALTH FEEDBACK ---")] 
    [Space(10)] 
    [SerializeField] private Volume _playerHealthVolume;
    [SerializeField] private float _lerpTimeVolume;
    [SerializeField] private float _intensityIncrement;

    private Coroutine _showDamageScreen;
    private Coroutine _hideDamageScreen;
    
    //GETTERS && SETTERS//
    public int CurrentHealth => currentHealth;
    public int RequiredHealth => requiredHealth;

    ////////////////////////////////
    
    private void Awake()
    {
        _playerScriptStorage = GetComponent<PlayerScriptStorage>();
        healthSlider = GetComponentInChildren<Slider>();
        _playerHealthVolume = GameObject.FindWithTag("PlayerHealthVolume").GetComponent<Volume>();
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10);
        }
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
            return;
        }

        if (_showDamageScreen != null)
        {
            StopCoroutine(_showDamageScreen);
            _showDamageScreen = null;
        }
        
        if (_hideDamageScreen != null)
        { 
            StopCoroutine(_hideDamageScreen); 
            _hideDamageScreen = null;
        }
        
        _showDamageScreen = StartCoroutine(ShowDamageScreen_Coroutine());
        _intensityIncrement += 0.1f;
    }
    
    private IEnumerator ShowDamageScreen_Coroutine()
    {
        float time = 0f;
        DirectionalBlur db;
        Vignette vignette;
        _playerHealthVolume.profile.TryGet(out db);
        _playerHealthVolume.profile.TryGet(out vignette);
        
        while (vignette.intensity.value < 0.5f + _intensityIncrement)
        {
            db.intensity.value = Mathf.Lerp(db.intensity.value, 8f, time);
            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, 0.5f + _intensityIncrement, time);
            Debug.Log("Showing Damage Screen");

            time += _lerpTimeVolume * Time.deltaTime;
            yield return null;
        }

        _hideDamageScreen = StartCoroutine(HideDamageScreen_Coroutine(db, vignette));
    }

    private IEnumerator HideDamageScreen_Coroutine(DirectionalBlur db, Vignette vignette)
    {
        float time = 0f;
        yield return new WaitForSeconds(3f);
        while (db.intensity.value > 0.1f)
        {
            db.intensity.value = Mathf.Lerp(db.intensity.value, 0, time);
            
            
            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, 0f, time);

            time += _lerpTimeVolume * Time.deltaTime;
            
            
            yield return null;
        }

        Debug.Log("STOP SHOWING DAMAGE SCREEN");
        _intensityIncrement = 0f;
    }

    public void AddHealth(int healthToRecovery)
    {
        currentHealth += healthToRecovery;
        currentHealth = Mathf.Clamp(currentHealth, 0, 100);
        healthSlider.value = currentHealth;
        _playerScriptStorage.Animator.SetFloat("Health", currentHealth/100f);
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
