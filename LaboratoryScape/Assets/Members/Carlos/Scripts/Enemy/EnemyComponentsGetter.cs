using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyComponentsGetter : MonoBehaviour
{
    //Variables
    [SerializeField] private Rigidbody[] _rigidbody;
    
    
    //GETTER & SETTERS//

    public Rigidbody[] Rigidbody
    {
        get => _rigidbody;
        set => _rigidbody = value;
    }

    ////////////////////////////////////////////////

    void Awake()
    {
        _rigidbody = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody enemyCollider in _rigidbody)
        {
            enemyCollider.AddComponent<EnemyShootCollisionDetector>();
            enemyCollider.GetComponent<EnemyShootCollisionDetector>().EnemyScriptsStorage = GetComponent<EnemyScriptsStorage>();
        }
    }
}
