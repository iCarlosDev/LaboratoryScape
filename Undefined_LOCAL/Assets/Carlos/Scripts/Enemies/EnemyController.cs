using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] protected CharacterController _characterController;
    [SerializeField] protected EnemyDespossess _enemyDespossess;

    [Header("--- MOVEMENT PARAMETERS ---")] 
    [Space(10)] 
    [SerializeField] protected float currentSpeed;
    [SerializeField] protected float walkSpeed;
    [SerializeField] private float sprintSpeed;

    [Header("--- JUMP PARAMETERS ---")] 
    [Space(10)] 
    [SerializeField] private float gravity;
    [SerializeField] private float jumpHeight;

    [Header("--- GROUND CHECK ---")] 
    [Space(10)]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance;
    [SerializeField] private bool isGrounded;

    private Vector3 velocity;

    private void Awake()
    {
        _enemyDespossess = GetComponent<EnemyDespossess>();
    }

    public virtual void Update()
    {
        Movement();
        Jump();
        Sprint();
    }

    private void Movement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        _characterController.Move(move.normalized * currentSpeed * Time.deltaTime);
    }

    private void Jump()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
        
        velocity.y += gravity * Time.deltaTime;

        velocity = new Vector3(_characterController.velocity.x, velocity.y, _characterController.velocity.z);
        _characterController.Move(velocity * Time.deltaTime);
    }

    public virtual void Sprint()
    {
        if (isGrounded && Input.GetAxis("Vertical") > 0f)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                currentSpeed = sprintSpeed;
            }

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                currentSpeed = walkSpeed;
            }   
        }
        else
        {
            currentSpeed = walkSpeed;
        }
        
    }
    
}
