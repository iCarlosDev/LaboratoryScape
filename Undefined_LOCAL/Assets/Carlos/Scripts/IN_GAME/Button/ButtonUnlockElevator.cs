using System;
using System.Collections;
using System.Collections.Generic;
using EPOOutline;
using TMPro;
using UnityEngine;

public class ButtonUnlockElevator : MonoBehaviour, I_Interact
{
    [SerializeField] private ElevatorManager elevatorManager;
    
    [SerializeField] private Animator _animator;
    [SerializeField] private bool isActivated;
    [SerializeField] private bool canPress;
    
    ////INTERACT INTERFACE////
    public Outlinable outliner { get; set; }

    public bool IsActivated => isActivated;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        elevatorManager = FindObjectOfType<ElevatorManager>();
        outliner = GetComponent<Outlinable>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canPress)
        {
            PressButton();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            PressButton();
        }
    }

    [ContextMenu("Press Elevator Button")]
    private void PressButton()
    {
        if (isActivated) return;
        
        isActivated = true;
        _animator.SetTrigger("PressButton");
        elevatorManager.CheckElevatorUnlocked();
        SetOultine(false);
        SetTextInteract(false);
    }
    
    public void SetOultine(bool shouldActivate)
    {
        outliner.enabled = shouldActivate;
    }

    public void SetTextInteract(bool shouldShow)
    {
        Level1Manager.instance.InteractCanvas.SetActive(shouldShow);
        Level1Manager.instance.InteractCanvas.GetComponentInChildren<TextMeshProUGUI>().text = $"Press E to Interact";
    }

    private void OnTriggerStay(Collider other)
    {
        if ((!other.CompareTag("PlayerRoot") && !other.CompareTag("PlayerRootFP")) || isActivated) return;

        if (Vector3.Distance(other.transform.position, transform.position) > 1f) return;

        canPress = true;
        SetOultine(true);
        SetTextInteract(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if ((!other.CompareTag("PlayerRoot") && !other.CompareTag("PlayerRootFP")) || isActivated) return;
        
        canPress = false;
        SetOultine(false);
        SetTextInteract(false);
    }
}
