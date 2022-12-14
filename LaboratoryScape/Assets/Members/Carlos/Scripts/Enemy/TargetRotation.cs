using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetRotation : MonoBehaviour
{
    [SerializeField] private Transform targetPos;

    private void Awake()
    {
        //enemyCamera = FindObjectOfType<FPMouseLook>().transform;
    }

    private void Update()
    {
        gameObject.transform.position = targetPos.position;
    }
}
