using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomControl : MonoBehaviour
{
    [SerializeField] private bool _isBeingSearched;

    public bool IsBeingSearched
    {
        get => _isBeingSearched;
        set => _isBeingSearched = value;
    }
}
