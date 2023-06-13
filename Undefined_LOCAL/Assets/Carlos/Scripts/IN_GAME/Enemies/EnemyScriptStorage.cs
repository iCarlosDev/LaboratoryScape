using System;
using System.Collections;
using System.Collections.Generic;
using EPOOutline;
using UnityEngine;

public class EnemyScriptStorage : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private FieldOfView _fieldOfView;
    [SerializeField] private Enemy_IA _enemyIa;
    [SerializeField] private Outlinable _outlinable;
    [SerializeField] private DoorCard _doorCard;

    //GETTERS && SETTERS//
    public Rigidbody Rigidbody => _rigidbody;
    public FieldOfView FieldOfView => _fieldOfView;
    public Enemy_IA EnemyIa => _enemyIa;
    public Outlinable Outlinable => _outlinable;
    public DoorCard DoorCard => _doorCard;

    ///////////////////////////////////////////////

    private void Awake()
    {
        _fieldOfView = GetComponentInChildren<FieldOfView>();
        _enemyIa = GetComponent<Enemy_IA>();
        _outlinable = GetComponent<Outlinable>();
        _doorCard = GetComponent<DoorCard>();
    }
}
