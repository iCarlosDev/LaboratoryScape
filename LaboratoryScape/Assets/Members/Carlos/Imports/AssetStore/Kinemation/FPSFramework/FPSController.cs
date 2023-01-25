// Designed by Kinemation, 2022

using System.Collections;
using System.Collections.Generic;
using Kinemation.FPSFramework.Runtime.Core;
using UnityEngine;

namespace Demo.Scripts.Runtime
{
    public class FPSController : MonoBehaviour
    {
        private PauseMenuController pauseMenuController;
        
        [Header("FPS Framework")]
        [SerializeField] private CoreAnimComponent coreAnimComponent;
        
        [Header("Character Controls")]
        [SerializeField] private Transform cameraBone;
        [SerializeField] private float crouchHeight;
        [SerializeField] private float sensitivity;

        [Header("Movement")] 
        [SerializeField] private bool shouldMove;
        [SerializeField] private float walkSpeed;
        [SerializeField] private float sprintSpeed;
        [SerializeField] private float crouchSpeed;
        [SerializeField] private float currentSpeed;
        [SerializeField] private CharacterController controller;
        [SerializeField] private Animator animator;

        [SerializeField] private List<Weapon> weapons;
        private RecoilAnimation _recoilAnimation;

        private Vector2 _playerInput;
        private Vector2 _smoothMoveInput;

        private int _index;

        private float _fireTimer = -1f;
        private int _bursts;
        private bool _aiming;
        private bool _reloading;

        private float _lowerCapsuleOffset;
        private CharAnimData _charAnimData;
        private static readonly int Sprint = Animator.StringToHash("sprint");
        private static readonly int Crouch1 = Animator.StringToHash("crouch");
        private static readonly int Moving = Animator.StringToHash("moving");
        private static readonly int MoveX = Animator.StringToHash("moveX");
        private static readonly int MoveY = Animator.StringToHash("moveY");
        
        [Header("--- POSSESSION ---")] 
        [Space(10)] 
        [SerializeField] private bool canBePossess;

        [Header("--- IA PARAMETERS ---")] 
        [Space(10)] 
        [SerializeField] private float moveX;
        [SerializeField] private float moveY;
        [SerializeField] private bool isIA;
        [SerializeField] private bool shouldAttack;

        [SerializeField] private EnemyScriptsStorage _enemyScriptsStorage;
        [SerializeField] private bool weaponBlockFlag;
        [SerializeField] private bool shouldCancelSprint;
        
        //GETTER & SETTER//
        public Transform CameraBone => cameraBone;
        public bool CanBePossess => canBePossess;
        public List<Weapon> Weapons => weapons;
        public int Index => _index;
        public float MoveX1
        {
            get => moveX;
            set => moveX = value;
        }
        public float MoveY1
        {
            get => moveY;
            set => moveY = value;
        }
        public bool IsIa
        {
            get => isIA;
            set => isIA = value;
        }
        public bool ShouldAttack
        {
            get => shouldAttack;
            set => shouldAttack = value;
        }
        public EnemyScriptsStorage EnemyScriptsStorage => _enemyScriptsStorage;
        public bool WeaponBlockFlag
        {
            get => weaponBlockFlag;
            set => weaponBlockFlag = value;
        }
        public bool aiming
        {
            get => _aiming;
            set => _aiming = value;
        }

        //////////////////////////////////////////////

        private void Start()
        {
            //Application.targetFrameRate = 120;

            _lowerCapsuleOffset = controller.center.y - controller.height / 2f;

            coreAnimComponent = GetComponent<CoreAnimComponent>();

            animator = GetComponent<Animator>();
            _recoilAnimation = GetComponent<RecoilAnimation>();

            controller = GetComponent<CharacterController>();

            _enemyScriptsStorage = GetComponent<EnemyScriptsStorage>();

            pauseMenuController = FindObjectOfType<PauseMenuController>();

            walkSpeed = 1.7f;
            sprintSpeed = walkSpeed * 2f;
            crouchSpeed = walkSpeed * 0.7f;
            currentSpeed = walkSpeed;

            EquipWeapon();
        }
        
        private void EquipWeapon()
        {
            var gun = weapons[_index];
            
            _bursts = gun.burstAmount;
            _recoilAnimation.Init(gun.recoilData, gun.fireRate, gun.fireMode);
            coreAnimComponent.OnGunEquipped(gun.gunData);

            //animator.Play(gun.poseName);
            gun.gameObject.SetActive(true);
        }
        
        private void ChangeWeapon()
        {
            int newIndex = _index;
            newIndex++;
            if (newIndex > weapons.Count - 1)
            {
                newIndex = 0;
            }

            weapons[_index].gameObject.SetActive(false);
            _index = newIndex;
            
            EquipWeapon();
        }

        public void ToggleAim()
        {
            if (!aiming && _charAnimData.movementState == FPSMovementState.Sprinting)
            {
                return;
            }
            
            _aiming = !_aiming;

            if (_aiming)
            {
                _charAnimData.actionState = FPSActionState.Aiming;
                _enemyScriptsStorage.WeaponPoseDetector.IsBlocked = false;
                _enemyScriptsStorage.WeaponPoseDetector.gameObject.SetActive(false);
                _enemyScriptsStorage.AimColliderDetector.gameObject.SetActive(true);
            }
            else
            {
                _charAnimData.actionState = FPSActionState.None;
                _enemyScriptsStorage.AimColliderDetector.IsColliding = false;
                _enemyScriptsStorage.AimColliderDetector.gameObject.SetActive(false);
                _enemyScriptsStorage.WeaponPoseDetector.gameObject.SetActive(true);
            }
            
            _recoilAnimation.isAiming = _aiming;
        }
        
        public void ChangeScope()
        {
            coreAnimComponent.OnSightChanged(GetGun().GetScope());
        }

        private void Fire()
        {
            GetGun().OnFire();
            _recoilAnimation.Play();
        }

        public void OnFirePressed()
        {
            if (_enemyScriptsStorage.Weapon.CurrentAmmo > 0)
            {
                Fire();
                _bursts = GetGun().burstAmount - 1; 
                _fireTimer = 0f;
            }
            else
            {
                OnFireReleased();
            }
        }

        private Weapon GetGun()
        {
            return weapons[_index];
        }

        public void OnFireReleased()
        {
            _enemyScriptsStorage.EnemyIaDecisions.FirstTimeShooting = false;
            _recoilAnimation.Stop();
            _fireTimer = -1f;
        }

        private void SprintPressed()
        {
            if (_charAnimData.poseState == FPSPoseState.Crouching)
            {
                return;
            }
            
            if (aiming)
            {
                ToggleAim();   
            }

            shouldCancelSprint = true;
            
            _charAnimData.movementState = FPSMovementState.Sprinting;
            _charAnimData.actionState = FPSActionState.None;

            currentSpeed = sprintSpeed;
            animator.SetBool(Sprint, true);
        }
        
        private void SprintReleased()
        {
            if (_charAnimData.poseState == FPSPoseState.Crouching)
            {
                return;
            }
            
            _charAnimData.movementState = FPSMovementState.Walking;

            currentSpeed = walkSpeed;
            animator.SetBool(Sprint, false);
        }

        private void Crouch()
        {
            var height = controller.height;
            height *= crouchHeight;
            controller.height = height;
            controller.center = new Vector3(0f, _lowerCapsuleOffset + height / 2f, 0f);
            currentSpeed = crouchSpeed;
            
            _charAnimData.poseState = FPSPoseState.Crouching;
            animator.SetBool(Crouch1, true);
        }

        private void Uncrouch()
        {
            var height = controller.height;
            height /= crouchHeight;
            controller.height = height;
            controller.center = new Vector3(0f, _lowerCapsuleOffset + height / 2f, 0f);
            currentSpeed = walkSpeed;

            _charAnimData.poseState = FPSPoseState.Standing;
            animator.SetBool(Crouch1, false);
        }

        private void ProcessActionInput()
        {
            _charAnimData.leanDirection = 0;

            if (_aiming)
            {
                _charAnimData.actionState = FPSActionState.Aiming;
            }
            
            if (Input.GetKeyDown(KeyCode.LeftShift) && (moveX > 0.1f || moveY > 0.1f))
            {
                SprintPressed();
            }

            if (Input.GetKeyUp(KeyCode.LeftShift) && _charAnimData.movementState == FPSMovementState.Sprinting)
            {
                SprintReleased();
            }
            
            if (Input.GetKeyDown(KeyCode.F))
            {
                ChangeWeapon();
            }
            
            if (weaponBlockFlag)
            {
                if (_enemyScriptsStorage.WeaponPoseDetector.IsBlocked || EnemyScriptsStorage.AimColliderDetector.IsColliding)
                {
                    _charAnimData.actionState = FPSActionState.Ready;
                    OnFireReleased();
                }
                else
                {
                    _charAnimData.actionState = FPSActionState.None;
                    weaponBlockFlag = false;
                }
            }
            
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                ToggleAim();
            }

            if (moveX.Equals(0f) && moveY.Equals(0f) && shouldCancelSprint)
            {
               SprintReleased();
               shouldCancelSprint = false;
            }

            if (_charAnimData.movementState == FPSMovementState.Sprinting)
            {
                return;
            }

            if (Input.GetKey(KeyCode.Q))
            {
                _charAnimData.leanDirection = 1;
            }
            else if (Input.GetKey(KeyCode.E))
            {
                _charAnimData.leanDirection = -1;
            }
            
            if (_charAnimData.actionState != FPSActionState.Ready)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    GetGun().Reload();
                }

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    OnFirePressed();
                }
            
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    OnFireReleased();
                }

                if (Input.GetKeyDown(KeyCode.V))
                {
                    ChangeScope();
                }
            
                if (Input.GetKeyDown(KeyCode.B) && _aiming)
                {
                    if (_charAnimData.actionState == FPSActionState.PointAiming)
                    {
                        _charAnimData.actionState = FPSActionState.Aiming;
                    }
                    else
                    {
                        _charAnimData.actionState = FPSActionState.PointAiming;
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                if (_charAnimData.poseState == FPSPoseState.Standing)
                {
                    Crouch();
                }
                else
                {
                    Uncrouch();
                }
            }

            /*if (Input.GetKeyDown(KeyCode.H))
            {
                if (_charAnimData.actionState == FPSActionState.Ready)
                {
                    _charAnimData.actionState = FPSActionState.None;
                }
                else
                {
                    _charAnimData.actionState = FPSActionState.Ready;
                    OnFireReleased();
                }
            }*/
        }

        public void ChangePose()
        {
            _charAnimData.actionState = FPSActionState.None;
        }

        private void ProcessLookInput()
        {
            float deltaMouseX = Input.GetAxis("Mouse X") * sensitivity;
            float deltaMouseY = -Input.GetAxis("Mouse Y") * sensitivity;
            
            _playerInput.x += deltaMouseX;
            _playerInput.y += deltaMouseY;
            
            _playerInput.x = Mathf.Clamp(_playerInput.x, -90f, 90f);
            _playerInput.y = Mathf.Clamp(_playerInput.y, -90f, 90f);

            _charAnimData.deltaAimInput = new Vector2(deltaMouseX, deltaMouseY);

            if (shouldMove)
            {
                transform.Rotate(Vector3.up * deltaMouseX);
            }
        }

        public void UpdateFiring()
        {
            if (_recoilAnimation.fireMode != FireMode.Semi && _fireTimer >= 60f / GetGun().fireRate)
            {
                if (_enemyScriptsStorage.Weapon.CurrentAmmo > 0)
                {
                    Fire();
                }
                else
                {
                    OnFireReleased();
                }

                if (_recoilAnimation.fireMode == FireMode.Burst)
                {
                    _bursts--;

                    if (_bursts == 0)
                    {
                        _fireTimer = -1f;
                        OnFireReleased();
                    }
                    else
                    {
                        _fireTimer = 0f;
                    }
                }
                else
                {
                    _fireTimer = 0f;
                }
            }

            if (_fireTimer >= 0f)
            {
                _fireTimer += Time.deltaTime;
            }

            _charAnimData.recoilAnim = new LocRot(_recoilAnimation.OutLoc,
                Quaternion.Euler(_recoilAnimation.OutRot));
        }

        private void UpdateMovement()
        {
            _charAnimData.moveInput = new Vector2(moveX, moveY);

            _smoothMoveInput.x = CoreToolkitLib.Glerp(_smoothMoveInput.x, moveX, 7f);
            _smoothMoveInput.y = CoreToolkitLib.Glerp(_smoothMoveInput.y, moveY, 7f);

            bool moving = Mathf.Abs(moveX) >= 0.4f || Mathf.Abs(moveY) >= 0.4f;
            
            animator.SetBool(Moving, moving);    
            animator.SetFloat(MoveX, _smoothMoveInput.x);
            animator.SetFloat(MoveY, _smoothMoveInput.y);
                
            Vector3 move = transform.right * moveX + transform.forward * moveY;
            controller.Move(move * currentSpeed * Time.deltaTime);
        }

        private void UpdateAnimValues()
        {
            coreAnimComponent.SetCharData(_charAnimData);
        }

        private void Update()
        {
            if (!isIA && !pauseMenuController.ShouldPause)
            {
                ProcessActionInput();   
                ProcessLookInput();
            }
            else
            {
                if (shouldAttack)
                {
                    _charAnimData.actionState = FPSActionState.None;
                }
                else
                {
                    _charAnimData.actionState = FPSActionState.Ready;
                    OnFireReleased();
                }
            }
            
            UpdateFiring();
            UpdateMovement();
            UpdateAnimValues();
        }

        private void UpdateCameraRotation()
        {
            var rootBone = coreAnimComponent.GetRootBone();
            cameraBone.rotation =
                rootBone.rotation * Quaternion.Euler(_playerInput.y, shouldMove ? 0f : _playerInput.x, 0f);
        }
        
        private void LateUpdate()
        {
            UpdateCameraRotation();
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
            }
        }
    }
}