using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeselectButtonNotPressed : MonoBehaviour
{
    [SerializeField] private EventSystem eventSystem;

    private void Awake()
    {
        eventSystem = FindObjectOfType<EventSystem>();
    }

    public void RemoveCurrentSelected()
    {
        eventSystem.SetSelectedGameObject(null);
    }
}
