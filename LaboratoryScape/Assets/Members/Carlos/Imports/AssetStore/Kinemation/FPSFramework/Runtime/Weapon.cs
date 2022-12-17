using System.Collections;
using System.Collections.Generic;
using Kinemation.FPSFramework.Runtime.Core;
using UnityEngine;

namespace Demo.Scripts
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private List<Transform> scopes;
        public Transform leftHandTarget;
        [SerializeField] private Transform muzzle;

        public RecoilAnimData recoilData;
        public GunAimData gunAimData;
        public Vector3 handsOffset;
        public LocRotSpringData springData;

        public FireMode fireMode;
        public float fireRate;
        public int burstAmount;
        public string poseName;
        public bool isShootgun;
        public bool canShoot;
        private Animator _animator;
        private int _scopeIndex;

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
            //Ray muzzleRay = new Ray(muzzle.position, muzzle.forward);
            RaycastHit hitedObject = new RaycastHit();
            //Physics.Raycast(muzzleRay, out hitedObject, 100);

            bool didHit = Physics.Raycast(muzzle.localPosition, muzzle.forward, out hitedObject);

            //Debug.Log(hitedObject.transform.name);
            
            Debug.DrawRay(muzzle.position, muzzle.forward * 100, Color.red, 4f);
            PlayFireAnim();

            if (didHit)
            {
                GameObject od = hitedObject.collider.gameObject;

                foreach (Rigidbody rig in od.GetComponentsInChildren<Rigidbody>())
                {
                    rig.AddForce(muzzle.forward, ForceMode.Impulse);
                }
                
                foreach (Rigidbody rig in od.GetComponentsInChildren<Rigidbody>())
                {
                    rig.AddForce(muzzle.forward, ForceMode.Impulse);   
                }
                
                foreach (Rigidbody rig in od.GetComponentsInChildren<Rigidbody>())
                {
                    rig.AddForce(muzzle.forward, ForceMode.Impulse);   
                }
            }
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
