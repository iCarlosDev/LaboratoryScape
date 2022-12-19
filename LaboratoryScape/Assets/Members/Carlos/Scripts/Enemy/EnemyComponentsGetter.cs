using System.Collections;
using System.Collections.Generic;
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
    }
}
