using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointControl : MonoBehaviour
{
    [SerializeField] private bool _isOcuped;

    public bool IsOcuped
    {
        get => _isOcuped;
        set => _isOcuped = value;
    }
}
