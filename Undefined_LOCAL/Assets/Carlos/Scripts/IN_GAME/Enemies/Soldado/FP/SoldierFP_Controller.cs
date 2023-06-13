using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoldierFP_Controller : EnemyController
{
    [Space(30)]
    [Header("___SOLDIER SCRIPT___")]
    [Space(10)]
    [SerializeField] private Transform arms;
    
    [Header("--- ANIMATOR ---")] 
    [Space(10)] 
    [SerializeField] private Animator _animator;
    
    [Header("--- UI PARAMETERS ---")] 
    [Space(10)] 
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject weaponHud;

    [Header("--- FIRE PARAMETERS ---")] 
    [Space(10)]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Transform shootPrefab;
    [SerializeField] private Transform _shotgunCanon;
    [SerializeField] private TextMeshProUGUI ammoTMP;
    [SerializeField] private LayerMask layerToDetect;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private int ammoCapacity;
    [SerializeField] private int maxAmmo;
    [SerializeField] private int currentAmmo;
    [SerializeField] private int numRays;
    [SerializeField] private int spreadAngle;
    [SerializeField] private bool canShoot;
    [SerializeField] private bool isShotgun;
    
    [Header("--- AIM PARAMETERS ---")] 
    [Space(10)]
    [SerializeField] private Transform aimPivot;
    [SerializeField] private float smoothTime;
    
    [Header("--- RELOAD PARAMETERS ---")] 
    [Space(10)]
    [SerializeField] private bool canReload;
    [SerializeField] private bool isReloading;
    [SerializeField] private bool cancelReload;
    
    [Header("--- CAMERA SHAKE ---")] 
    [Space(10)]
    [SerializeField] private float magnitude;
    [SerializeField] private float roughnes;
    [SerializeField] private float fadeIn;
    [SerializeField] private float fadeOut;
    
    [SerializeField] private GameObject _bulletPrefab;
    
    //GETTER && SETTERS//
    public Transform CameraPivot => cameraPivot;

    ///////////////////////////////

    private void Start()
    {
        canShoot = true;
        ammoTMP.text = $"{currentAmmo}/{maxAmmo}";
    }

    public override void OnEnable()
    {
        base.OnEnable();
        
        arms.gameObject.SetActive(_enemyDespossess.Enemy?.EnemyType != Enemy_IA.EnemyType_Enum.Scientist);
        
        CheckAmmo();
        crosshair.SetActive(_enemyDespossess.Enemy?.EnemyType != Enemy_IA.EnemyType_Enum.Scientist);
        weaponHud.SetActive(_enemyDespossess.Enemy?.EnemyType != Enemy_IA.EnemyType_Enum.Scientist);

        maxAmmo = 30;
        currentAmmo = ammoCapacity;
    }

    private void OnDisable()
    {
        canReload = false;
        isReloading = false;
        cancelReload = false;

        if (currentAmmo != 0)
        {
            canShoot = true;
        }
    }

    public override void Update()
    {
        base.Update();

        if (_enemyDespossess.Enemy is { EnemyType: Enemy_IA.EnemyType_Enum.Scientist } || PauseMenuManager.instance.IsPaused) return;

        MovementAnimationControll();
        
        //Si apretamos el click Izquierdo y podemos disparar llamaremos al método "Fire";
        if (Input.GetButtonDown("Fire1") && canShoot && currentAmmo != 0)
        {
            Fire();
            AudioManager.instance.Play("EnemyShoot");
            Debug.LogWarning("SHOOOOT");
        }
        //Si nos quedamos sin balas en el cargador y disparamos recargará automaticamente;
        else if (Input.GetButtonDown("Fire1") && canShoot && canReload)
        {
            Reload();
        }
        //Si disparamos mientras recargamos cancelaremos la recarga;
        else if (Input.GetButtonDown("Fire1") && isReloading)
        {
            cancelReload = true;
        }

        //Si mantenemos el click Derecho llamaremos al método "AimIn" si no al método "AimOut";
        if (Input.GetButton("Fire2"))
        {
            AimIn();
        }
        else
        {
            AimOut();
        }

        //Si apretamos la tecla "R", podemos recargar y no estamos ya recargando, podremos recargar;
        if (Input.GetKeyDown(KeyCode.R) && canReload && !isReloading)
        {
            Reload();
        }
    }

    //Método para controlar el Sprint;
    public override void Sprint()
    {
        //Si no estoy apuntando y no estoy recargando podré sprintar;
        if (!AimIn() && !isReloading)
        {
            base.Sprint();
        }
        else
        {
            currentSpeed = walkSpeed;
        }
    }

    //Método para controlar las animaciones básicas;
    private void MovementAnimationControll()
    {
        //Si el character está en el suelo asignaremos su velocidad en el animator;
        if (_characterController.isGrounded)
        {
            _animator.SetFloat("CharacterMovement", _characterController.velocity.magnitude);
        }
        
        //Asignamos la currentSpeed en el animator;
        _animator.SetFloat("CharacterSpeed", currentSpeed);
    }

    #region - FIRE -
    
    //Método para disparar;
    private void Fire()
    {
        //Activamos el trigger "Fire";
        _animator.SetTrigger("Fire");
        canShoot = false;

        //Aplicamos un shake a la cámara para dar efecto de disparo;
        EZCameraShake.CameraShaker.Instance.ShakeOnce(magnitude, roughnes, fadeIn, fadeOut);
        
        CallNearSoldiers(10);

        for (int i = 0; i < numRays; i++)
        {
            // Calcular la dirección del rayo con dispersión
            Quaternion spreadRotation = Quaternion.Euler(Random.Range(-spreadAngle, spreadAngle), Random.Range(-spreadAngle, spreadAngle), 0f);
            Vector3 rayDirection = spreadRotation * cameraPivot.forward;

            Debug.DrawRay(cameraPivot.position, rayDirection * 10, Color.red, 3f);
            GameObject bullet = Instantiate(_bulletPrefab, _shotgunCanon.position, _shotgunCanon.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(rayDirection * 100, ForceMode.Impulse);

            RaycastHit hit;
            Ray ray = new Ray(cameraPivot.position, rayDirection);

            //Cuando disparamos lanzamos un rayo que da información de con que ha impactado;
            if (Physics.Raycast(ray, out hit, 25f, layerToDetect, QueryTriggerInteraction.Ignore))
            {
                Instantiate(shootPrefab, hit.point, hit.transform.rotation);
                
                //Si impacta con un collider del enemy...;
                if (hit.collider.CompareTag("EnemyCollider"))
                {
                    Debug.Log(hit.collider.name);
                    
                    //Llamamos al método que tendrá el collider con el que impactamos;
                    hit.collider.SendMessage("OnDamage", hit);
                }
            }
        }

        #region - CHECK AMMO -
        
        currentAmmo--;
        CheckAmmo();
        
        #endregion
    }

    private void CheckAmmo()
    {
        ammoTMP.text = $"{currentAmmo}/{maxAmmo}";

        //Si disparamos y tenemos munición de reserva podrémos recargar;
        if (currentAmmo < ammoCapacity && maxAmmo != 0)
        {
            canReload = true;
        }
    }
    
    private void CallNearSoldiers(float sphereRadius)
    {
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, sphereRadius, enemyLayer);
        
        foreach (Collider collider in colliderArray)
        {
            if (collider.GetComponent<Soldier_IA>())
            {
                collider.GetComponent<Soldier_IA>().IsPlayerDetected = true;
                collider.GetComponent<Soldier_IA>().StartCoroutine(collider.GetComponent<Soldier_IA>().DetectPlayer());
            }
            else
            {
                collider.GetComponent<Scientist_IA>().IsPlayerDetected = true;
                collider.GetComponent<Scientist_IA>().StartCoroutine(collider.GetComponent<Scientist_IA>().DetectPlayer());
            }
        }
    }

    //Método que se llama al hacerse la animación de disparo;
    public void ResetShot()
    {
        canShoot = true;
    }
    #endregion

    #region - AIM -

    //Método para apuntar;
    private bool AimIn()
    {
        //Movemos la posición de los brazos con un smooth hacia el pivote de apuntado;
        arms.position = Vector3.Lerp(arms.position, aimPivot.position, smoothTime * Time.deltaTime);
        //Bajamos la magnitud del shake para que la escopeta no atraviese la cámara;
        magnitude = 0.25f;

        return Input.GetButton("Fire2");
    }

    //Método para desapuntar;
    private void AimOut()
    {
        //Movemos la posición de los brazos con un smooth hacia su posición inicial;
        arms.localPosition = Vector3.Lerp(arms.localPosition, Vector3.zero, smoothTime * Time.deltaTime);
        //Dejamos el valor de la magnitud del shake al inicial;
        magnitude = 1f;
    }

    #endregion

    #region - RELOAD -

    //Método para recargar;
    private void Reload()
    {
        canShoot = false;
        isReloading = true;
        _animator.SetTrigger("Reload");
        _animator.SetBool("StopReload", false);
        
        crosshair.SetActive(false);
    }

    //Método que se llama al hacerse la animación de recarga;
    public void ResetReload()
    {
        CheckReload();
        ammoTMP.text = $"{currentAmmo}/{maxAmmo}";
    }

    //Método para recargar de distinta forma dependiendo del arma en uso;
    private void CheckReload()
    {
        //Comprobamos que no es una escopeta;
        if (!isShotgun)
        {
            //Si no hemos vaciado completamente el cargador...;
            if (currentAmmo > 0)
            {
                //Si tenemos mas o igual munición de reserva que el cargador...;
                if (maxAmmo >= ammoCapacity)
                {
                    maxAmmo -= ammoCapacity - currentAmmo;
                    currentAmmo = ammoCapacity;
                }
                //Si tenemos menos munición de reserva que en el cargador...;
                else if (maxAmmo < ammoCapacity)
                {
                    //Si la munición actual + la de reserva hace más que la que cabe en el cargador;
                    if (currentAmmo + maxAmmo > ammoCapacity)
                    {
                        maxAmmo -= ammoCapacity - currentAmmo;
                        currentAmmo = ammoCapacity;
                    }
                    //Si la munición actual + la de reserva hace menos que la que cabe en el cargador;
                    else
                    {
                        currentAmmo += maxAmmo;
                        maxAmmo = 0;
                    }
                }
            }
            //Si hemos vaciado completamente el cargador...;
            else if (currentAmmo.Equals(0))
            {
                //Si tenemos mas o igual munición de reserva que el cargador...;
                if (maxAmmo >= ammoCapacity)
                {
                    currentAmmo = ammoCapacity;
                    maxAmmo -= ammoCapacity;
                }
                //Si no tenemos mas o igual munición de reserva que el cargador...;
                else
                {
                    currentAmmo += maxAmmo;
                    maxAmmo = 0;
                }
            }
        }
        //Si es una escopeta...;
        else
        {
            //Siempre que haya munición de reserva recargaremos;
            if (maxAmmo != 0)
            {
                currentAmmo++;
                maxAmmo--;  
            }
        }

        //Si llegamos a la máxima capacidad del cargador, nos quedamos sin munición de reserva o cancelamos la recarga, dejaremos de recargar;
        if (currentAmmo == ammoCapacity || maxAmmo == 0 || cancelReload)
        {
            _animator.SetBool("StopReload", true);
            if (currentAmmo == ammoCapacity || maxAmmo == 0)
            {
                canReload = false; 
            }
            isReloading = false;
            canShoot = true;
            cancelReload = false;
            
            crosshair.SetActive(true);
        }
    }

    #endregion
}
