using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FusiblesControl : MonoBehaviour
{
    [SerializeField] private PlayerScriptStorage playerScriptStorage;
    [SerializeField] private Transform animPos;
    [SerializeField] private float timeToGoAnimPos;
    [SerializeField] private bool canInteract;

    private void Awake()
    {
        playerScriptStorage = FindObjectOfType<PlayerScriptStorage>();
        animPos = transform.GetChild(1);
    }

    private void Start()
    { 
        PutAnimPosOnFloor();   
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canInteract)
        {
            StartCoroutine(GoAnimationPosition_Coroutine());
        }
    }

    private void PutAnimPosOnFloor()
    {
        Ray ray = new Ray(animPos.position, -animPos.up);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            animPos.position = hit.point;
        }
    }
    
    private IEnumerator GoAnimationPosition_Coroutine()
    {
        while (Vector3.Distance(animPos.position,  playerScriptStorage.transform.position) > 0.01f)
        {
            playerScriptStorage.gameObject.transform.position = Vector3.Lerp(playerScriptStorage.gameObject.transform.position, animPos.position, timeToGoAnimPos);
            playerScriptStorage.transform.rotation = Quaternion.Lerp(playerScriptStorage.transform.rotation, animPos.rotation, timeToGoAnimPos);
            yield return null;
        }
        
        DestroyFusibles();
    }

    private void DestroyFusibles()
    {
        playerScriptStorage.Animator.SetTrigger("RomperFusibles");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerRoot"))
        {
            canInteract = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerRoot"))
        {
            canInteract = false;
        }
    }
}
