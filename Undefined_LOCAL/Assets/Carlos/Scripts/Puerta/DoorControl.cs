using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorControl : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float timeLeftCloseDoor;

    [SerializeField] private DoorCardStatus doorCardStatusEnum;
    private enum DoorCardStatus
    {
        PurpleCard,
        YellowCard,
        RedCard,
        NoCard
    }
    
    private Coroutine closeDoor;
    
    //GETTERS && SETTERS//
    public Animator Animator => _animator;
    
    /////////////////////////////////////////////

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OpenDoor()
    {
        // Nombre del estado que estamos buscando
        string stateName = "DoorOpen";
        // Obtener la información del estado actual del Animator
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        // Obtener el nombre del estado actual
        string currentStateName = stateInfo.shortNameHash.ToString();

        // Verificar si el estado que estamos buscando es el estado actual
        if (currentStateName.Equals(Animator.StringToHash(stateName).ToString()) && stateInfo.normalizedTime > 0)
        {
            // El estado que estamos buscando se está ejecutando
            CloseDoor();
            return;
        }
        
        _animator.SetTrigger("DoorOpen");
    }
    
    private void CloseDoor()
    {
        if (closeDoor != null)
        {
            StopCoroutine(closeDoor);
            closeDoor = null;
        }
        
        closeDoor = StartCoroutine(CloseDoor_Coroutine());
    }

    private IEnumerator CloseDoor_Coroutine()
    {
        yield return new WaitForSeconds(timeLeftCloseDoor);
        _animator.SetTrigger("DoorClose");
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("PlayerRoot") || other.CompareTag("PlayerRootFP"))
        {
            if (doorCardStatusEnum == DoorCardStatus.NoCard)
            {
                OpenDoor(); 
            }

            if ((int)other.GetComponent<DoorCard>().DoorCardEnum == (int)doorCardStatusEnum)
            {
                OpenDoor();
            }
        }
    }
}
