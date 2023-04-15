using UnityEngine;

public class DoorCard : MonoBehaviour
{
    [SerializeField] private DoorCardStatus doorCardEnum;
    public enum DoorCardStatus
    {
        PurpleCard,
        YellowCard,
        RedCard,
        NoCard
    }
    
    //GETTERS && SETTERS//
    public DoorCardStatus DoorCardEnum
    {
        get => doorCardEnum;
        set => doorCardEnum = value;
    }
}
