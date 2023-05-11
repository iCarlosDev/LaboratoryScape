using System.Collections.Generic;
using UnityEngine;

public class DoorCard : MonoBehaviour
{
    [SerializeField] private List<DoorCardStatus> doorCardEnumList;
    public enum DoorCardStatus
    {
        BlueCard,
        RedCard,
        GreenCard,
        TutorialCard
    }
    
    //GETTERS && SETTERS//
    public List<DoorCardStatus> DoorCardEnumList
    {
        get => doorCardEnumList;
        set => doorCardEnumList = value;
    }
}
