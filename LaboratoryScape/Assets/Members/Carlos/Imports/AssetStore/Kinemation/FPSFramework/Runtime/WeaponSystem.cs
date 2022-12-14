using System.Collections.Generic;
using Kinemation.FPSFramework.Runtime.Core;
using Kinemation.FPSFramework.Runtime.Layers;
using UnityEngine;

namespace Demo.Scripts
{
    public class WeaponSystem : MonoBehaviour
    {
        [SerializeField] private List<Weapon> weapons;
        private RecoilAnimation _recoilAnimation;
        [SerializeField] private CoreAnimComponent coreAnimComponent;
        [SerializeField] private SwayLayer swayLayer;
        [SerializeField] private AdsLayer gunAnimLayer;
        [SerializeField] private RecoilLayer recoilAnimLayer;
        [SerializeField] private LeftHandIKLayer leftHandLayer;

        private Animator _animator;
        private int _index;

        private float _fireTimer = -1f;
        private int _bursts;
        private bool _aiming;
        private bool reloading;
        
        [SerializeField] private bool bUseInput = true;

        private void Start()
        {
            Application.targetFrameRate = 120;
            Cursor.visible = false;
        
            _animator = GetComponent<Animator>();
            _recoilAnimation = GetComponent<RecoilAnimation>();

            EquipWeapon();
        }

        private Weapon GetGun()
        {
            return weapons[_index];
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
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
        
            if (Input.GetKeyDown(KeyCode.Mouse0) && bUseInput)
            {
                Fire();
                _bursts = GetGun().burstAmount - 1;
                _fireTimer = 0f;
            }
        
            if (Input.GetKeyUp(KeyCode.Mouse0) && bUseInput)
            {
                CancelFire();
                _fireTimer = -1f;
            }

            if (_recoilAnimation.fireMode != FireMode.Semi && _fireTimer >= 60f / GetGun().fireRate)
            {
                Fire();

                if (_recoilAnimation.fireMode == FireMode.Burst)
                {
                    _bursts--;

                    if (_bursts == 0)
                    {
                        _fireTimer = -1f;
                        CancelFire();
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

            if (Input.GetKeyDown(KeyCode.Mouse1) && bUseInput)
            {
                ToggleAim();
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                gunAnimLayer.aimData.aimPoint = GetGun().GetScope();
            }

            if (Input.GetKeyDown(KeyCode.B) && _aiming)
            {
                gunAnimLayer.pointAiming = !gunAnimLayer.pointAiming;
            }
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                _animator.Play(reloading ? GetGun().poseName : "Reload");
                reloading = !reloading;
            }
            
            recoilAnimLayer.SetRecoilAnim(new LocRot(_recoilAnimation.OutLoc, Quaternion.Euler(_recoilAnimation.OutRot)));
        }

        private void ToggleAim()
        {
            _aiming = !_aiming;
            _recoilAnimation.isAiming = _aiming;
            gunAnimLayer.aiming = _aiming;
            
            if(!_aiming)
            {
                gunAnimLayer.pointAiming = false;
            }
        }

        private void Fire()
        {
            GetGun().OnFire();
            _recoilAnimation.Play();
        }

        private void CancelFire()
        {
            _recoilAnimation.Stop();
        }

        private void EquipWeapon()
        {
            var gun = weapons[_index];

            // Current fire mode
            _recoilAnimation.fireMode = gun.fireMode;
            _recoilAnimation.Init(gun.recoilData, gun.fireRate);
            gunAnimLayer.InitLayer(gun.gunAimData);
            gunAnimLayer.SetHandsOffset(gun.handsOffset);
            
            swayLayer.SetTargetSway(gun.springData);
            coreAnimComponent.SetMasterIKTarget(gun.gunAimData.pivotPoint);
            leftHandLayer.leftHandTarget = gun.leftHandTarget;

            _bursts = gun.burstAmount;
        
            _animator.Play(gun.poseName);
            gun.gameObject.SetActive(true);
        }
    }
}
