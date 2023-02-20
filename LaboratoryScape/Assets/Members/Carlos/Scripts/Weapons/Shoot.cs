using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
   //Variables
   [SerializeField] private Transform muzzle;

   // Update is called once per frame
    void Update()
    { 
        Fire();   
    }

    private void Fire()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            
        }
    }
}
