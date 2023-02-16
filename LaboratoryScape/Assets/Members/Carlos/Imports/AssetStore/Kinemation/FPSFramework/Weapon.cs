// Designed by Kinemation, 2022

using System.Collections;
using System.Collections.Generic;
using Kinemation.FPSFramework.Runtime.Core;
using TMPro;
using UnityEngine;

namespace Demo.Scripts.Runtime
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private EnemyScriptsStorage enemyScriptsStorage;
        
        [SerializeField] private List<Transform> scopes;
        [SerializeField] public WeaponAnimData gunData;
        [SerializeField] public RecoilAnimData recoilData;
        [SerializeField] public CoreAnimComponent _coreAnimComponent;
        
        public FireMode fireMode;
        public float fireRate;
        public int burstAmount;

        private Animator _animator;
        private int _scopeIndex;
        
        /////////////////////////////////////////////
        /////////////////////////////////////////////
        
        [SerializeField] private Transform muzzle;
        [SerializeField] private LayerMask enemyColider;
        [SerializeField] private float impactForce;

        [Header("--- BLOOD ---")]
        public Light DirLight;
        public GameObject BloodAttach;
        public GameObject[] BloodFX;
        public Vector3 direction;

        [Header("--- DAMAGE ---")] 
        [Space(10)]
        [SerializeField] private int weaponDamage;
        
        [Header("--- SHOOT ---")] 
        [Space(10)]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private int ammoCapacity;
        [SerializeField] private int maxAmmo;
        [SerializeField] private int currentAmmo;
        [SerializeField] private float probabilidadDeFallar = 0.25f;
        [SerializeField] private bool isShootgun;
        [SerializeField] private bool shouldShoot;
        
        [Header("--- CANVAS ---")] 
        [Space(10)]
        [SerializeField] private TextMeshProUGUI ammoTMP;

        public int Counter;

        //GETTERS && SETTERS//
        public Transform Muzzle => muzzle;
        public int CurrentAmmo => currentAmmo;
        public int MaxAmmo => maxAmmo;
        public TextMeshProUGUI AmmoTMP
        {
            get => ammoTMP;
            set => ammoTMP = value;
        }
        public bool ShouldShoot => shouldShoot;

        /////////////////////////////////////////////

        private void Start()
        {
            enemyScriptsStorage = GetComponentInParent<EnemyScriptsStorage>();
            _animator = GetComponentInParent<Animator>();
            _audioSource = GetComponent<AudioSource>();
            _coreAnimComponent = GetComponentInParent<CoreAnimComponent>();
            shouldShoot = true;
            
            maxAmmo = 300;
            currentAmmo = ammoCapacity;
        }

        public Transform GetScope()
        {
            _scopeIndex++;
            _scopeIndex = _scopeIndex > scopes.Count - 1 ? 0 : _scopeIndex;
            return scopes[_scopeIndex];
        }

        private IEnumerator waitToShoot()
        {
            yield return new WaitForSeconds(1f);
            shouldShoot = true;
        }

        public void OnFire()
        {
            if (isShootgun && !shouldShoot)
            {
                return;
            }
            
            currentAmmo--;

            if (ammoTMP != null)
            {
                ammoTMP.text = $"{currentAmmo} / {maxAmmo}";
            }

            RaycastHit hit = new RaycastHit();
            Ray ray = new Ray(muzzle.position, muzzle.forward);

            if (Physics.Raycast(ray, out hit, 100f, enemyColider, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.CompareTag("EnemyColliders"))
                {
                   SpawnBlood(hit);
                   
                   hit.collider.GetComponent<Rigidbody>().AddForce(-hit.normal * impactForce); 
                   hit.collider.SendMessage("hit");
                }
                
                if (hit.collider.CompareTag("Player"))
                {
                    ShootPlayerProbability();
                }
            }

            Debug.DrawRay(muzzle.position, muzzle.forward * 100, Color.red, 4);
            PlayFireAnim();

            _audioSource.PlayOneShot(_audioSource.clip);

            if (isShootgun)
            {
                StartCoroutine(waitToShoot());
                shouldShoot = false;
            }
        }

        public void AnimReload()
        {
            if (currentAmmo < ammoCapacity)
            {
                _coreAnimComponent.UseIK = false;
                _animator.Play("Reloading", 2);
            }
        }

        public void Reload()
        {
            if (currentAmmo > 0)
            {
                if (maxAmmo >= ammoCapacity)
                {
                    maxAmmo -= ammoCapacity - currentAmmo;
                    currentAmmo = ammoCapacity;
                }
                else if (maxAmmo < ammoCapacity)
                {
                    if (currentAmmo + maxAmmo > ammoCapacity)
                    {
                        maxAmmo -= ammoCapacity - currentAmmo;
                        currentAmmo = ammoCapacity;
                    }
                    else
                    {
                        currentAmmo += maxAmmo;
                        maxAmmo = 0;
                    }
                }
            }
            else if (currentAmmo.Equals(0))
            {
                if (maxAmmo >= ammoCapacity)
                {
                    currentAmmo = ammoCapacity;
                    maxAmmo -= ammoCapacity;
                }
                else
                {
                    currentAmmo += maxAmmo;
                    maxAmmo = 0;
                }
            }

            if (!enemyScriptsStorage.FPSController.IsIa)
            {
                ammoTMP.text = $"{currentAmmo} / {maxAmmo}";
            }
            
            enemyScriptsStorage.FPSController.IsReloading = false;
        }

        private void ShootPlayerProbability()
        {
            float aleatorio = Random.Range(0f, 1f);
                    
            if (aleatorio < probabilidadDeFallar)
            {
                // el disparo ha fallado
                Debug.Log("El disparo ha fallado");
            }
            else
            {
                Debug.Log("Player Disparado!");
                PlayerScriptsStorage.instance.PlayerHealth.TakeDamage(1);
            }
        }

        private void SpawnBlood(RaycastHit hit)
        {
            // var randRotation = new Vector3(0, Random.value * 360f, 0);
            //var dir = CalculateAngle(Vector3.forward, hit.normal);
            float angle = Mathf.Atan2(-hit.normal.x, -hit.normal.z) * Mathf.Rad2Deg + 180;

            var effectIdx = Random.Range(0, BloodFX.Length);
            if (effectIdx == BloodFX.Length) effectIdx = 0;

            var instance = Instantiate(BloodFX[effectIdx], hit.point, Quaternion.Euler(0, angle + 90, 0));
            effectIdx++;

            //var settings = instance.GetComponent<BFX_BloodSettings>();
            //settings.FreezeDecalDisappearance = true;
            //settings.LightIntensityMultiplier = DirLight.intensity;

            var attachBloodInstance = Instantiate(BloodAttach);
            var bloodT = attachBloodInstance.transform;
            bloodT.position = hit.point;
            bloodT.localRotation = Quaternion.identity;
            bloodT.localScale = Vector3.one * Random.Range(0.75f, 1.2f);
            bloodT.LookAt(hit.point + hit.normal, this.direction);
            bloodT.Rotate(90, 0, 0);
            bloodT.transform.parent = hit.transform;
            Destroy(attachBloodInstance, 10f);
            Destroy(instance, 13f);
        }

        private void PlayFireAnim()
        {
            if (_animator == null)
            {
                return;
            }
            _animator.Play("Fire", 0, 0f);
        }
    }
}
