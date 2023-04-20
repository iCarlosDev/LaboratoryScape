using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RejillaAnimationController : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Animator _animator;
    [SerializeField] private SphereCollider _sphereCollider;
    [SerializeField] private Transform AnimPosition0;
    [SerializeField] private Transform AnimPosition1;
    [SerializeField] private float timeToGoAnimPos;
    [SerializeField] private bool canInteract;

    private Coroutine goAnimationPosition;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _sphereCollider = GetComponent<SphereCollider>();
        AnimPosition0 = transform.GetChild(0);
        AnimPosition1 = transform.GetChild(1);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canInteract && goAnimationPosition == null)
        {
            float playerSide = transform.position.z - playerMovement.transform.position.z;
            if ((playerSide) > 0)
            { 
                goAnimationPosition = StartCoroutine(GoAnimationPosition_Coroutine(AnimPosition0 ,playerSide));
            }
            else
            {
                goAnimationPosition = StartCoroutine(GoAnimationPosition_Coroutine(AnimPosition1 ,playerSide));   
            }

            playerMovement.CanMove = false;
            Debug.Log($"{Vector3.Distance(AnimPosition0.position, playerMovement.transform.position)}");
        }
    }

    private IEnumerator GoAnimationPosition_Coroutine(Transform animPos, float playerSide)
    {
        while (Vector3.Distance(animPos.position, playerMovement.transform.position) > 0.01f)
        {
            playerMovement.gameObject.transform.position = Vector3.Lerp(playerMovement.gameObject.transform.position, animPos.position, timeToGoAnimPos);
            playerMovement.transform.rotation = Quaternion.Lerp(playerMovement.transform.rotation, animPos.rotation, timeToGoAnimPos);
            yield return null;
        }
        
        DoAnimation(playerSide);
    }

    private void DoAnimation( float playerSide)
    {
        _sphereCollider.enabled = false;

        if ((playerSide) > 0)
        {
            playerMovement.PlayerScriptStorage.Animator.SetTrigger("ArrancarRejilla");
            _animator.SetTrigger("ArrancarRejilla");
        }
        else
        {
            playerMovement.PlayerScriptStorage.Animator.SetTrigger("EmpujarRejilla");
            _animator.SetTrigger("EmpujarRejilla");
        }

        canInteract = false;
    }

    public void CalculatePhysics()
    {
        _animator.enabled = false;
        
        Rigidbody rigidbody = GetComponentInChildren<Rigidbody>();
        rigidbody.transform.parent = null;
        rigidbody.isKinematic = false;
        
        rigidbody.AddForce(rigidbody.velocity * 10f, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerRoot"))
        {
            canInteract = true;

            if (playerMovement == null)
            {
                playerMovement = other.GetComponent<PlayerMovement>();
            }
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
