using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("--- SWAY PARAMETERS ---")] 
    [Space(10)] 
    [SerializeField] private float smooth;
    [SerializeField] private float swayMultiplier;

    private void Update()
    {
        SwayWeapon();
    }

    private void SwayWeapon()
    {
        //Obtenemos el Input del Mouse;
        float mouseX = Input.GetAxisRaw("Mouse X") * swayMultiplier;
        float mouseY = Input.GetAxisRaw("Mouse Y") * swayMultiplier;

        //Calculamos la rotación del target;
        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        //Rotamos;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth + Time.deltaTime);
    }
}
