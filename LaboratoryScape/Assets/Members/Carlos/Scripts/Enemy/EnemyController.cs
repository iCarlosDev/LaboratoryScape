using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    //Variables
    [SerializeField] private CharacterController _characterController;
    
    [Header("--- WALK ---")]
    [Space(10)]
    [SerializeField] private float speed;
    
    [Header("--- JUMP ---")]
    [Space(10)]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundDistance;
    [SerializeField] private float jumpHeight;
    [SerializeField] private bool isGrounded;

    [Header("--- SPRINT ---")]
    [Space(10)]
    [SerializeField] private Camera cameraFP;
    [Range(60f, 70f)]
    [SerializeField] private float fov;
    [SerializeField] private bool isSprinting;

    [Header("--- CROUCH ---")] 
    [Space(10)] 
    [SerializeField] private bool isCrouched;
    
    [Header("--- SLIDE ---")] 
    [Space(10)] 
    [SerializeField] private bool isSliding;

    private Vector3 velocity;
    private float gravity = -9.81f;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        cameraFP = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        cameraFP.fieldOfView = fov;
        
        Walk();
        Jump();
        Sprint();
        Crouch();
        Slide();
    }

    private void Walk()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }
        
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        _characterController.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        _characterController.Move(velocity * Time.deltaTime);
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void Sprint()
    {
        fov = Mathf.Clamp(fov, 60f, 70f);

        if (!isCrouched && !isSliding)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                isSprinting = true;
                speed = 6f;
            }
            else
            {
                isSprinting = false;
                speed = 3f;
            }
        }
    }

    private void Crouch()
    {
        if (!isSliding && !isSprinting)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                isSprinting = false;
                isCrouched = true;
                _characterController.height = 1f;
                speed = 1f;
            }
            else
            {
                isCrouched = false;
                _characterController.height = 2f;
                speed = 3f;
            } 
        }
    }

    private void Slide()
    {
        if (!isCrouched)
        {
            if (Input.GetKey(KeyCode.LeftControl) && isSprinting)
            { 
                isSliding = true; 
                _characterController.Move(transform.forward * (speed) * Time.deltaTime);
                _characterController.height = 1f; 
                speed -= Time.deltaTime * 8f;

                if (speed <= 3)
                {
                    isSprinting = false;
                    isSliding = false;
                    _characterController.height = 2f; 
                }
            }
        }
    }
}
