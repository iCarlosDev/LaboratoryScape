using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeselectButtonNotPressed : MonoBehaviour
{
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private Button _button;

    private void Awake()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        _button = GetComponent<Button>();
    }

    public void RemoveCurrentSelected()
    {
        _button.OnDeselect(eventSystem.MDummyData);
    }
}
