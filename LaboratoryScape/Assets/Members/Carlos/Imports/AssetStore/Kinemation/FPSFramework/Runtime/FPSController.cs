using System;
using EPOOutline;
using Kinemation.FPSFramework.Runtime.Core;
using Kinemation.FPSFramework.Runtime.Layers;
using UnityEngine;

namespace Demo.Scripts
{
    public class FPSController : MonoBehaviour
    {
        [SerializeField] private SwayLayer swayLayer;
        [SerializeField] private LookLayer lookAnimLayer;
        [SerializeField] private Transform rootBone;
        [SerializeField] private Transform cameraBone;
        
        private Vector2 _playerInput;
        [SerializeField] private float sensitivity;

        [Header("Movement")] [SerializeField] private bool shouldMove;
        [SerializeField] private CharacterController controller;
        [SerializeField] private float speed = 10f;
        [SerializeField] private Animator animator;

        private Vector2 smoothMoveInput;
        
        [Header("--- POSSESSION ---")] 
        [Space(10)] 
        [SerializeField] private bool canBePossess;

        [Header("--- OTHER ---")] 
        [SerializeField] private Outlinable outlinable;
        
        //////////////////////////////////
        public Transform CameraBone => cameraBone;

        public bool CanBePossess => canBePossess;

        public Outlinable Outlinable => outlinable;

        private void Awake()
        {
            outlinable = GetComponent<Outlinable>();
        }

        private void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit(0);
            }
            
            float deltaMouseX = Input.GetAxis("Mouse X") * sensitivity;
            float deltaMouseY = -Input.GetAxis("Mouse Y") * sensitivity;
            
            _playerInput.x += deltaMouseX;
            _playerInput.y += deltaMouseY;
            
            if (Input.GetKey(KeyCode.Q))
            {
                lookAnimLayer.SetLeanInput(1);
            }
            else if (Input.GetKey(KeyCode.E))
            {
                lookAnimLayer.SetLeanInput(-1);
            }
            else
            {
                lookAnimLayer.SetLeanInput(0);
            }

            swayLayer.deltaInput = new Vector2(deltaMouseX, deltaMouseY);
            lookAnimLayer.SetAimRotation(new Vector2(shouldMove ? 0f : deltaMouseX, deltaMouseY));

            if (shouldMove)
            {
                float moveX = Input.GetAxis("Horizontal");
                float moveY = Input.GetAxis("Vertical");

                smoothMoveInput.x = CoreToolkitLib.Glerp(smoothMoveInput.x, moveX, 7f);
                smoothMoveInput.y = CoreToolkitLib.Glerp(smoothMoveInput.y, moveY, 7f);
                
                animator.SetFloat("moveX", smoothMoveInput.x);
                animator.SetFloat("moveY", smoothMoveInput.y);
                
                Vector3 move = transform.right * moveX + transform.forward * moveY;
                controller.Move(move * speed * Time.deltaTime);
                
                transform.Rotate(Vector3.up * deltaMouseX);
            }
        }

        // Update after core anim component, so we get a smooth rotation
        private void LateUpdate()
        {
            _playerInput.x = Mathf.Clamp(_playerInput.x, -90f, 90f);
            _playerInput.y = Mathf.Clamp(_playerInput.y, -90f, 90f);
            cameraBone.rotation = rootBone.rotation * Quaternion.Euler(_playerInput.y, shouldMove ? 0f : _playerInput.x, 0f);
        }
        
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                canBePossess = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                CarlosSceneManager.instance.EnemiesInRangeList.Add(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                canBePossess = false;
                CarlosSceneManager.instance.EnemiesInRangeList.Remove(this);
                outlinable.enabled = false;
            }
        }
    }
}