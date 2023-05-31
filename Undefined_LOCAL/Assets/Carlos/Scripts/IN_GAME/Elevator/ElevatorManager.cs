using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElevatorManager : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Material _lightOnMaterial;
    [SerializeField] private Transform _elevatorLightsParent;
    [SerializeField] private MeshRenderer[] _elevatorLights;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _elevatorLights = _elevatorLightsParent.GetComponentsInChildren<MeshRenderer>();
    }

    public void CheckElevatorUnlocked()
    {
        foreach (var elevatorLight in _elevatorLights)
        {
            if (!elevatorLight.CompareTag("ElevatorLightOn"))
            {
                elevatorLight.material = _lightOnMaterial;
                elevatorLight.tag = "ElevatorLightOn";
                break;
            }
        }
        
        if (Level1Manager.instance.ButtonUnlockElevatorList.Any(button => !button.IsActivated)) return;

        _animator.SetTrigger("OpenElevator");
    }
}
