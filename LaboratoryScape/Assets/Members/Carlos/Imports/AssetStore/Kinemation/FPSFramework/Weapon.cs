// Designed by Kinemation, 2022

using System.Collections.Generic;
using Kinemation.FPSFramework.Runtime.Core;
using UnityEngine;

namespace Demo.Scripts.Runtime
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private List<Transform> scopes;
        [SerializeField] public WeaponAnimData gunData;
        [SerializeField] public RecoilAnimData recoilData;
        
        public FireMode fireMode;
        public float fireRate;
        public int burstAmount;

        private Animator _animator;
        private int _scopeIndex;
        
        /////////////////////////////////////////////
        /////////////////////////////////////////////
        
        public Transform muzzle;
        public LayerMask enemyColider;
        public float impactForce;
        
        /////////////////////////////////////////////
        /////////////////////////////////////////////

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        public Transform GetScope()
        {
            _scopeIndex++;
            _scopeIndex = _scopeIndex > scopes.Count - 1 ? 0 : _scopeIndex;
            return scopes[_scopeIndex];
        }
        
        public void OnFire()
        {
            RaycastHit hit = new RaycastHit();
            Ray ray = new Ray(muzzle.position, muzzle.forward);

            if (Physics.Raycast(ray, out hit, 100f, enemyColider, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.CompareTag("EnemyColliders"))
                {
                    hit.collider.GetComponentInParent<EnemyDespossess>().EnemyDie();
                    hit.collider.GetComponent<Rigidbody>().AddForce(-hit.normal * impactForce);
                }
            }

            Debug.DrawRay(muzzle.position, muzzle.forward * 100, Color.red, 4);
            PlayFireAnim();
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
