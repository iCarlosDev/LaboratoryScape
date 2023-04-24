using System;
using System.Collections;
using Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerMovement : MonoBehaviour
{
    //Variables
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private PlayerScriptStorage _playerScriptStorage;

    [Header("--- CAMERA SETTINGS ---")] 
    [Space(10)] 
    [SerializeField] private CinemachineFreeLook _cinemachineFreeLook;
    
    [Header("--- MOVEMENT ---")]
    [Space(10)]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private bool canMove;
    [SerializeField] private bool isInConduct;
    [SerializeField] private float horizontal;
    [SerializeField] private float vertical;
    [SerializeField] private float speed;
    [SerializeField] private float turnSmoothTime;
    [SerializeField] private Transform baseSkeleton;
    
    [SerializeField] private Vector3 refBaseSkeleton;
    
    private float turnSmoothVelocity;

    [Header("--- JUMP ---")] 
    [Space(10)] 
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private bool isGrounded;
    [SerializeField] private float sphereRadius;
    [SerializeField] private float gravity;
    [SerializeField] private float jumpForce;
    
    [Header("--- IDLE_2 PARAMETERS ---")] 
    [Space(10)] 
    [SerializeField] private float timeInIdle;

    //GETTERS & SETTERS//
    public bool CanMove
    {
        get => canMove;
        set => canMove = value;
    }
    public bool IsInConduct => isInConduct;
    public PlayerScriptStorage PlayerScriptStorage => _playerScriptStorage;

    /////////////////////////////////////////

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _playerScriptStorage = GetComponent<PlayerScriptStorage>();
        _cinemachineFreeLook = GetComponentInChildren<CinemachineFreeLook>();
        playerCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>().transform;
        groundCheck = transform.GetChild(2);
    }

    private void Start()
    {
        speed = 2f;
        canMove = true;
    }

    public void SetSensitivityOptions()
    {
        _cinemachineFreeLook.m_YAxis.m_MaxSpeed = OptionsManager.instance.MouseSensitivity * 10f;
        _cinemachineFreeLook.m_YAxis.m_InvertInput = OptionsManager.instance.InvertVertical;
        _cinemachineFreeLook.m_XAxis.m_MaxSpeed = OptionsManager.instance.MouseSensitivity * 1000f;
        _cinemachineFreeLook.m_XAxis.m_InvertInput = OptionsManager.instance.InvertHorizontal;
    }

    public void EnablePlayerMovement()
    {
        canMove = true;
    }

    private void Update()
    {
        if (PauseMenuManager.instance.IsPaused || !canMove) return;
        
        CalculateGravity();

        //Si el player tiene vida se hará la lógica restante;
        if (_playerScriptStorage.PlayerHealth.CurrentHealth > 0)
        {
            Movement();
            
            //Comprobamos si el player está en el suelo y en movimiento;
            if (isGrounded && _characterController.velocity.magnitude >= 0.1f)
            {
                //Comprobamos que la vida de el player sea mayor a la vida requerida;
                if (_playerScriptStorage.PlayerHealth.CurrentHealth > _playerScriptStorage.PlayerHealth.RequiredHealth)
                {
                    Sprint(); 
                }
                else
                {
                    speed = 1f;
                }
            }
            else
            {
                speed = 2f;
                _playerScriptStorage.Animator.SetFloat("SpeedAnimation", 2.25f);
            }

            //Comprobamos que la vida de el player sea mayor a la vida requerida;
            if (_playerScriptStorage.PlayerHealth.CurrentHealth > _playerScriptStorage.PlayerHealth.RequiredHealth)
            {
                Jump();
            }
        }
    }

    private void Movement()
    {
        //Guardo en estas variables las teclas WASD;
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        //Recogemos los valores WASD en positivo y los guardo como "direction";
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        
        //Si el jugador está en movimiento...;
        if(direction.magnitude >= 0.1f)
        {
            //Recogemos el ángulo del input entre la dirección "X" y "Z" y la proyección del angulo entre esos 2 valores lo convertimos en grados para saber hacia donde mira nuestro player;
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
            //Smoothea la posición actual a la posición obtenida en "targetAngle";
            float angle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, turnSmoothTime);
            //rota el personaje en el eje "Y";
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            //guardamos donde rota en "Y" el player por su eje "Z" (dando así que siempre donde mire el player será al frente);
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            //Movemos el player hacia el frente;
            _characterController.Move(moveDir.normalized * speed * Time.deltaTime);
            
            _playerScriptStorage.Animator.SetBool("IsWalking", true);
            timeInIdle = 0f;
        }
        else
        {
            MakeIdle2();
            _playerScriptStorage.Animator.SetBool("IsWalking", false);
        }
    }
    
    private void CalculateGravity()
    {
        //El eje "Y" irá progresivamente a 0;
        velocity.y += gravity * Time.deltaTime;
        
        //Movemos al personaje en el eje "Y" para tener gravedad;
        _characterController.Move(velocity * Time.deltaTime);
    }

    private void Sprint()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed = 4f;
            _playerScriptStorage.Animator.SetFloat("SpeedAnimation", 3f);
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = 2f;
            _playerScriptStorage.Animator.SetFloat("SpeedAnimation", 2.25f);
        }
    }

    private void Jump()
    {
        //Seteamos el bool con una esfera invisible triggered;
        isGrounded = Physics.CheckSphere(groundCheck.position, sphereRadius, groundMask);
        _playerScriptStorage.Animator.SetBool("IsGrounded", isGrounded);
        
        //si estamos en el suelo y no estamos cayendo el eje "Y" será "-2";
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        //Si presionamos "Espacio" y estamos en el suelo...;
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            //igualamos nuestro eje "Y" a los valores de la raíz cuadrada (el resultado debería ser valor positivo);
            velocity.y = Mathf.Sqrt(jumpForce * -2 * gravity);
            _playerScriptStorage.Animator.SetTrigger("Jump");
        }
    }

    private void MakeIdle2()
    {
        timeInIdle += Time.deltaTime;
        
        //Si pasas 10 sec quieto...;
        if (timeInIdle >= 10f)
        {
            //Si sale un número mayor de 0.7...;
            if (Random.value > 0.7f)
            {
                _playerScriptStorage.Animator.SetTrigger("Idle2");
            }
            
            //reseteamos el tiempo en idle;
            timeInIdle = 0f;
        }
    }

    public void ActivarAlarmaPorFusibles()
    {
        FindObjectOfType<Enemy_IA>().AlarmActivated();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ConductCollider"))
        {
            _playerScriptStorage.VirtualCamera.Priority = 11;
            isInConduct = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ConductCollider"))
        {
            _playerScriptStorage.VirtualCamera.Priority = 9;
            isInConduct = false;
        }
    }
}
