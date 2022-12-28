using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScriptsStorage : MonoBehaviour
{
    public static PlayerScriptsStorage instance;

    [SerializeField] private PlayerHealth _playerHealth;
    
    //GETTERS && SETTERS//
    public PlayerHealth PlayerHealth => _playerHealth;
    
    //////////////////////////////////////////
    
    private void Awake()
    {
        instance = this;

        _playerHealth = GetComponent<PlayerHealth>();
    }
}
