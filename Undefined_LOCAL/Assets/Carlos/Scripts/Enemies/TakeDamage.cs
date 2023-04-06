using System;
using UnityEngine;

public class TakeDamage : MonoBehaviour
{
    [SerializeField] private EnemyScriptStorage _enemyScriptStorage;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private int damage;

    private void Awake()
    {
        _enemyScriptStorage = GetComponentInParent<EnemyScriptStorage>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Método que se llama por (SendMessage) desde el SoldierFP_Controller;
    /// </summary>
    /// <param name="hit"></param>
    /// Actualiza la vida del NPC;
    public void OnDamage(RaycastHit hit)
    {
        _enemyScriptStorage.EnemyIa.TakeDamage(damage, hit, _rigidbody);
    }
}
