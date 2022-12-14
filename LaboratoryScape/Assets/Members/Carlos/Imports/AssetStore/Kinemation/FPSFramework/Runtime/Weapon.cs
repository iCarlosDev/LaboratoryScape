using System.Collections.Generic;
using Kinemation.FPSFramework.Runtime.Core;
using UnityEngine;

namespace Demo.Scripts
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private List<Transform> scopes;
        public Transform leftHandTarget;
        
        public RecoilAnimData recoilData;
        public GunAimData gunAimData;
        public Vector3 handsOffset;
        public LocRotSpringData springData;
        
        public FireMode fireMode;
        public float fireRate;
        public int burstAmount;
        public string poseName;
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
