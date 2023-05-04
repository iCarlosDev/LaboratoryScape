using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonUnlockElevator : MonoBehaviour
{
    [SerializeField] private ElevatorManager elevatorManager;
    
    [SerializeField] private Animator _animator;
    [SerializeField] private bool isActivated;
    [SerializeField] private bool canPress;

    public bool IsActivated => isActivated;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        elevatorManager = FindObjectOfType<ElevatorManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canPress)
        {
            PressButton();
        }
    }

    private void PressButton()
    {
        if (isActivated) return;
        
        isActivated = true;
        _animator.SetTrigger("PressButton");
        elevatorManager.CheckElevatorUnlocked();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerRoot") || other.CompareTag("PlayerRootFP"))
        {
            canPress = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerRoot") || other.CompareTag("PlayerRootFP"))
        {
            canPress = false;
        }
    }
}
