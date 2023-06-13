using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFPLook : MonoBehaviour
{
    [SerializeField] private PlayerScriptStorage _playerScriptStorage;
    
    [Header("--- FP LOOK ---")] 
    [Space(10)] 
    [SerializeField] private Transform playerBody;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float xRotation;
    [SerializeField] private float _maxView;

    private void Awake()
    {
        _playerScriptStorage = GetComponentInParent<PlayerScriptStorage>();
        playerBody = transform.parent;
    }

    private void Update()
    {
        if (_playerScriptStorage.PlayerMovement.IsInConduct)
        {
            PlayerLookFP();
        }
    }

    private void PlayerLookFP()
    {
        float MouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float MouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= MouseY;
        xRotation = Mathf.Clamp(xRotation, -_maxView, _maxView);
        
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * MouseX);
    }

    public void SetSensitivityOptions()
    {
        mouseSensitivity = OptionsManager.instance.MouseSensitivity * (1000f / 2);
    }
}
