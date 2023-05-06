using UnityEngine;

public class DoorCard : MonoBehaviour
{
    [SerializeField] private DoorCardStatus doorCardEnum;
    public enum DoorCardStatus
    {
        BlueCard,
        RedCard,
        GreenCard,
        NoCard
    }
    
    //GETTERS && SETTERS//
    public DoorCardStatus DoorCardEnum
    {
        get => doorCardEnum;
        set => doorCardEnum = value;
    }
}
