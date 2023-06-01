using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FusiblesControl : MonoBehaviour
{
    [SerializeField] private PlayerScriptStorage playerScriptStorage;
    [SerializeField] private ParticleSystem _sparksParticles;
    [SerializeField] private Transform animPos;
    [SerializeField] private float timeToGoAnimPos;
    [SerializeField] private bool canInteract;

    private void Awake()
    {
        playerScriptStorage = FindObjectOfType<PlayerScriptStorage>();
        _sparksParticles = GetComponentInChildren<ParticleSystem>();
        animPos = transform.GetChild(1);
    }

    private void Start()
    { 
        _sparksParticles.gameObject.SetActive(false);
        PutAnimPosOnFloor();   
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canInteract)
        {
            StartCoroutine(GoAnimationPosition_Coroutine());
            playerScriptStorage.PlayerMovement.CanMove = false;
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

    [ContextMenu("Destroy Fusibles")]
    private void DestroyFusibles()
    {
        playerScriptStorage.Animator.SetTrigger("RomperFusibles");
        Invoke(nameof(ActivateParticles), 2.3f);
        
        foreach (DoorControl door in Level1Manager.instance.DoorsList)
        {
            door.BoxCollider.enabled = false;
            door.OpenDoor();
        }
    }

    public void ActivateParticles()
    {
        _sparksParticles.gameObject.SetActive(true);
        _sparksParticles.Play();
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
