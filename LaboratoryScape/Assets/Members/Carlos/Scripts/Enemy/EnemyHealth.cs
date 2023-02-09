using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private EnemyScriptsStorage _enemyScriptsStorage;
    
    //Variables
    [Header("--- HEALTH VALUES ---")]
    [Space(10)]
    [SerializeField] private int currentHealth;
    [SerializeField] private int maxHealth;
    [SerializeField] private bool alreadyDead;
    
    [Header("--- HEALTH PARAMETERS ---")]
    [Space(10)]
    [SerializeField] private TextMeshProUGUI health_TMP;

    [Header("--- DAMAGE VALUES ---")]
    [Space(10)]
    [SerializeField] private int headDamage;
    [SerializeField] private int bodyDamage;
    [SerializeField] private int extremitiesDamage;

    [Header("--- SOUND ---")] 
    [Space(10)] 
    [SerializeField] private AudioSource walkieTalkieAudioSource;
    [SerializeField] private AudioSource footStepsAudioSource;

    //GETTERS && SETTERS//
    public int HeadDamage => headDamage;
    public int BodyDamage => bodyDamage;
    public int ExtremitiesDamage => extremitiesDamage;
    public TextMeshProUGUI HealthTMP
    {
        get => health_TMP;
        set => health_TMP = value;
    }
    public int CurrentHealth => currentHealth;

    ////////////////////////////////////////////
    
    private void Awake()
    {
        _enemyScriptsStorage = GetComponent<EnemyScriptsStorage>();
    }

    private void Start()
    {
        maxHealth = 100;
        currentHealth = maxHealth;

        StartCoroutine(WalkieTalkieSoundCoroutine());
    }

    private void Update()
    {
        if (!alreadyDead)
        {
            IsDead();   
        }
    }

    public void FootStepsSound()
    {
        footStepsAudioSource.PlayOneShot(footStepsAudioSource.clip);
    }

    private IEnumerator WalkieTalkieSoundCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);
            int randomNumber = Random.Range(1, 3);
            Debug.Log(randomNumber);
            if (randomNumber == 2)
            {
                walkieTalkieAudioSource.Play(); 
            }
        }
    }

    private void IsDead()
    {
        if (currentHealth <= 0)
        {
            alreadyDead = true;
            _enemyScriptsStorage.EnemyDespossess.EnemyDie();   
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (health_TMP != null)
        { 
            health_TMP.text = $"{currentHealth}";
        }
    }
}
