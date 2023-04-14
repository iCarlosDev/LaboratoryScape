using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorControl : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float timeLeftCloseDoor;

    private Coroutine closeDoor;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OpenDoor()
    {
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
        if (other.CompareTag("Enemy") || other.CompareTag("PlayerRoot"))
        {
            OpenDoor();
        }
    }
}
