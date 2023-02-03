using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScriptsStorage : MonoBehaviour
{
    public static PlayerScriptsStorage instance;

    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private PlayerController _playerController;
    
    //GETTERS && SETTERS//
    public PlayerHealth PlayerHealth => _playerHealth;
    public PlayerController playerController => _playerController;

    //////////////////////////////////////////
    
    private void Awake()
    {
        instance = this;

        _playerHealth = GetComponent<PlayerHealth>();
        _playerController = GetComponent<PlayerController>();
    }
}
