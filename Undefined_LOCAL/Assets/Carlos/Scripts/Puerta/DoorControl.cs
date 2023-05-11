using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorControl : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private MeshRenderer emissiveMesh;
    [SerializeField] private Material blueCardMaterial;
    [SerializeField] private Material redCardMaterial;
    [SerializeField] private Material greenCardMaterial;
    [SerializeField] private Material tutorialCardMaterial;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private float timeLeftCloseDoor;

    [SerializeField] private DoorCardStatus doorCardStatusEnum;
    private enum DoorCardStatus
    {
        BlueCard,
        RedCard,
        GreenCard,
        TutorialCard,
        NoCard
    }
    
    private Coroutine closeDoor;
    
    //GETTERS && SETTERS//
    public Animator Animator => _animator;
    public BoxCollider BoxCollider
    {
        get => boxCollider;
        set => boxCollider = value;
    }

    /////////////////////////////////////////////

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        SetMaterialColorDoor();
    }

    private void SetMaterialColorDoor()
    {
        switch (doorCardStatusEnum)
        {
            case DoorCardStatus.BlueCard:
                emissiveMesh.material = blueCardMaterial;
                break;
            case DoorCardStatus.RedCard:
                emissiveMesh.material = redCardMaterial;
                break;
            case DoorCardStatus.GreenCard:
                emissiveMesh.material = greenCardMaterial;
                break;
            case DoorCardStatus.TutorialCard:
                emissiveMesh.material = tutorialCardMaterial;
                break;
            default:
                emissiveMesh.material = defaultMaterial;
                break;
        }
    }

    public void OpenDoor()
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
        if (!boxCollider.enabled) return;

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
                return;
            }

            DoorCard doorCard = other.GetComponent<DoorCard>();
            
            foreach (DoorCardStatus doorCardStatus in doorCard.DoorCardEnumList)
            {
                if ((int)doorCardStatus == (int)doorCardStatusEnum)
                {
                    OpenDoor();
                } 
            }
        }
    }
}
