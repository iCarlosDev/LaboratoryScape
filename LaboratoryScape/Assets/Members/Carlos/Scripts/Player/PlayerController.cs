using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Variables
    [SerializeField] private CharacterController _characterController;
    
    [Header("--- MOVEMENT ---")]
    [Space(10)]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float speed;
    [SerializeField] private float turnSmoothTime;
    
    private float turnSmoothVelocity;

    [Header("--- JUMP ---")] 
    [Space(10)] 
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private bool isGrounded;
    [SerializeField] private float groundDistance;
    [SerializeField] private float gravity;
    [SerializeField] private float jumpForce;
    
    //GETTERS & SETTERS//
    public Transform PlayerCamera => playerCamera;

    /////////////////////////////////////////

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        playerCamera = GameObject.Find("MainCamera").GetComponent<Camera>().transform;
        groundCheck = GameObject.Find("GroundCheck").transform;
    }

    private void Update()
    {
        Movement();
        Jump();
    }

    private void Movement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if(direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            _characterController.Move(moveDir.normalized * speed * Time.deltaTime);
        }
    }

    private void Jump()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2 * gravity);
        }
      
        velocity.y += gravity * Time.deltaTime;
        _characterController.Move(velocity * Time.deltaTime);
    }
}
