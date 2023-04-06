using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMouseLook : MonoBehaviour
{
    [Header("--- MOUSE PARAMETERS ---")] 
    [Space(10)] 
    [SerializeField] private float mouseSensitivity;

    [SerializeField] private Transform enemyFPBody;
    [SerializeField] private float xRotation;
    [SerializeField] private float maxYRotation;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        CameraRotation();
    }

    private void CameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxYRotation, maxYRotation);
        
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        enemyFPBody.Rotate(Vector3.up * mouseX);
    }
}
