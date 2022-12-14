// Designed by Kinemation, 2022

using Kinemation.FPSFramework.Runtime.Core;
using UnityEngine;

namespace Kinemation.FPSFramework.Runtime.Layers
{
    public class AdsLayer : AnimLayer
    {
        [SerializeField] private Vector3 handsOffset;
        [Header("SightsAligner")]
        [Range(0f, 1f)] public float aimLayerAlphaLoc;
        [Range(0f, 1f)] public float aimLayerAlphaRot;
        public GunAimData aimData;
        [SerializeField] private Transform aimTarget;
        public bool aiming;
        public bool pointAiming;
        
        private float _aimAlphaLayer = 0f;
        private float _pointAimAlphaLayer = 0f;
        private LocRot _smoothAimPoint;
        private LocRot _recoilAnim;

        public override void OnAnimUpdate()
        {
            ApplyAiming();
            ApplyPointAiming();
        }

        public void InitLayer(GunAimData gunAimData)
        {
            aimData = gunAimData;
        }

        public void SetHandsOffset(Vector3 offset)
        {
            handsOffset = offset;
        }

        public void CalculateAimData()
        {
            var stateName = aimData.target.stateName.Length > 0
                ? aimData.target.stateName
                : aimData.target.staticPose.name;

            if (rigData.animator != null)
            {
                rigData.animator.Play(stateName);
                rigData.animator.Update(0f);
            }
            
            // Cache the local data, so we can apply it without issues
            aimData.target.aimLoc = aimData.pivotPoint.InverseTransformPoint(aimTarget.position);
            aimData.target.aimRot = Quaternion.Inverse(aimData.pivotPoint.rotation) * rigData.rootBone.rotation;
        }

        private void ApplyAiming()
        {
            //Apply Aiming
            var masterTransform = rigData.masterDynamic.obj.transform;
            _aimAlphaLayer = CoreToolkitLib.GlerpLayer(_aimAlphaLayer, aiming && !pointAiming ? 1f : 0f, 
                aimData.aimSpeed);

            CoreToolkitLib.MoveInBoneSpace(rigData.rootBone, rigData.masterDynamic.obj.transform,
                handsOffset * (1f - _aimAlphaLayer));

            Vector3 scopeAimLoc = Vector3.zero;
            Quaternion scopeAimRot = Quaternion.identity;

            if (aimData.aimPoint != null)
            {
                scopeAimRot = Quaternion.Inverse(aimData.pivotPoint.rotation) * aimData.aimPoint.rotation;
                scopeAimLoc = -aimData.pivotPoint.InverseTransformPoint(aimData.aimPoint.position);
            }

            if (!_smoothAimPoint.position.Equals(scopeAimLoc))
            {
                _smoothAimPoint.position = CoreToolkitLib.Glerp(_smoothAimPoint.position, scopeAimLoc, aimData.aimSpeed);
            }

            if (!_smoothAimPoint.rotation.Equals(scopeAimRot))
            {
                _smoothAimPoint.rotation = CoreToolkitLib.Glerp(_smoothAimPoint.rotation, scopeAimRot, aimData.aimSpeed);
            }

            Vector3 addAimLoc = aimData.target.aimLoc;
            Quaternion addAimRot = aimData.target.aimRot * _smoothAimPoint.rotation;

            // Base Animation layer
            Vector3 baseLoc = masterTransform.position;
            Quaternion baseRot = masterTransform.rotation;

            CoreToolkitLib.MoveInBoneSpace(masterTransform, masterTransform, addAimLoc);
            masterTransform.rotation *= addAimRot;
            CoreToolkitLib.MoveInBoneSpace(masterTransform, masterTransform, _smoothAimPoint.position);

            addAimLoc = masterTransform.position;
            addAimRot = masterTransform.rotation;

            ApplyAbsAim(_smoothAimPoint.position, _smoothAimPoint.rotation);

            // Blend between Absolute and Additive
            masterTransform.position = Vector3.Lerp(masterTransform.position, addAimLoc, aimLayerAlphaLoc);
            masterTransform.rotation = Quaternion.Slerp(masterTransform.rotation, addAimRot, aimLayerAlphaRot);

            // Blend Between Non-Aiming and Aiming
            masterTransform.position = Vector3.Lerp(baseLoc, masterTransform.position, _aimAlphaLayer);
            masterTransform.rotation = Quaternion.Slerp(baseRot, masterTransform.rotation, _aimAlphaLayer);
        }

        private void ApplyPointAiming()
        {
            _pointAimAlphaLayer = CoreToolkitLib.GlerpLayer(_pointAimAlphaLayer, pointAiming && aiming ? 1f : 0f, 
                aimData.aimSpeed);
            
            CoreToolkitLib.MoveInBoneSpace(rigData.rootBone, rigData.masterDynamic.obj.transform,
                aimData.pointAimOffset.position * _pointAimAlphaLayer);

            var pointAimRot = Quaternion.Slerp(Quaternion.identity, aimData.pointAimOffset.rotation, 
                _pointAimAlphaLayer);
            
            CoreToolkitLib.RotateInBoneSpace(rigData.rootBone.rotation, rigData.masterDynamic.obj.transform,
                pointAimRot);
        }

        private void ApplyAbsAim(Vector3 loc, Quaternion rot)
        {
            Vector3 offset = -loc;
            
            rigData.masterDynamic.obj.transform.position = aimTarget.position;
            rigData.masterDynamic.obj.transform.rotation = rigData.rootBone.rotation * rot;
            CoreToolkitLib.MoveInBoneSpace(rigData.masterDynamic.obj.transform,
                rigData.masterDynamic.obj.transform, -offset);
        }
    }
}