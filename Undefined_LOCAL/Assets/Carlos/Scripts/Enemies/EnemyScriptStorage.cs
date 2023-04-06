using System;
using System.Collections;
using System.Collections.Generic;
using EPOOutline;
using UnityEngine;

public class EnemyScriptStorage : MonoBehaviour
{
    [SerializeField] private FieldOfView _fieldOfView;
    [SerializeField] private Enemy_IA _enemyIa;
    [SerializeField] private Outlinable _outlinable;

    //GETTERS && SETTERS//
    public FieldOfView FieldOfView => _fieldOfView;
    public Enemy_IA EnemyIa => _enemyIa;
    public Outlinable Outlinable => _outlinable;
    
    ///////////////////////////////////////////////

    private void Awake()
    {
        _fieldOfView = GetComponentInChildren<FieldOfView>();
        _enemyIa = GetComponent<Enemy_IA>();
        _outlinable = GetComponent<Outlinable>();
    }
}
