using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorManager : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void CheckElevatorUnlocked()
    {
        foreach (ButtonUnlockElevator button in Level1Manager.instance.ButtonUnlockElevatorList)
        {
            if (!button.IsActivated) return;
        }
        
        _animator.SetTrigger("OpenElevator");
    }
}
