// Designed by Kinemation, 2022

using Kinemation.FPSFramework.Runtime.Core;
using UnityEngine;

namespace Kinemation.FPSFramework.Runtime.Layers
{
    public class AdsLayer : AnimLayer
    {
        [Header("SightsAligner")]
        [Range(0f, 1f)] public float aimLayerAlphaLoc;
        [Range(0f, 1f)] public float aimLayerAlphaRot;
        [SerializeField] private Transform aimTarget;

        private float _aimAlphaLayer = 0f;
        private float _pointAimAlphaLayer = 0f;
        private LocRot _smoothAimPoint;
        private LocRot _recoilAnim;

        public override void OnAnimUpdate()
        {
            var dynamicMaster = GetMasterIK();
            
            Vector3 baseLoc = dynamicMaster.position;
            Quaternion baseRot = dynamicMaster.rotation;
            
            ApplyAiming();
            ApplyPointAiming();
            
            Vector3 postLoc = dynamicMaster.position;
            Quaternion postRot = dynamicMaster.rotation;

            dynamicMaster.position = Vector3.Lerp(baseLoc, postLoc, layerAlpha);
            dynamicMaster.rotation = Quaternion.Slerp(baseRot, postRot, layerAlpha);
        }

        public void CalculateAimData()
        {
            var aimData = GetGunData().gunAimData;
            
            var stateName = aimData.target.stateName.Length > 0
                ? aimData.target.stateName
                : aimData.target.staticPose.name;

            if (GetAnimator() != null)
            {
                GetAnimator().Play(stateName);
                GetAnimator().Update(0f);
            }
            
            // Cache the local data, so we can apply it without issues
            aimData.target.aimLoc = aimData.pivotPoint.InverseTransformPoint(aimTarget.position);
            aimData.target.aimRot = Quaternion.Inverse(aimData.pivotPoint.rotation) * GetRootBone().rotation;
        }

        private void ApplyAiming()
        {
            var aimData = GetGunData().gunAimData;
            
            bool bApplyAiming = GetActionState() == FPSActionState.Aiming &&
                                GetActionState() != FPSActionState.PointAiming;
            
            //Apply Aiming
            var masterTransform = GetMasterIK();
            _aimAlphaLayer = CoreToolkitLib.GlerpLayer(_aimAlphaLayer, bApplyAiming ? 1f : 0f, 
                aimData.aimSpeed);
            
            CoreToolkitLib.MoveInBoneSpace(GetRootBone(), GetMasterIK(),
                GetGunData().handsOffset * (1f - _aimAlphaLayer));

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
            var aimData = GetGunData().gunAimData;
            bool bApplyAiming = GetActionState() == FPSActionState.PointAiming;
            
            _pointAimAlphaLayer = CoreToolkitLib.GlerpLayer(_pointAimAlphaLayer, bApplyAiming ? 1f : 0f, 
                aimData.aimSpeed);
            
            CoreToolkitLib.MoveInBoneSpace(GetRootBone(), GetMasterIK(),
                aimData.pointAimOffset.position * _pointAimAlphaLayer);

            var pointAimRot = Quaternion.Slerp(Quaternion.identity, aimData.pointAimOffset.rotation, 
                _pointAimAlphaLayer);
            
            CoreToolkitLib.RotateInBoneSpace(GetRootBone().rotation, GetMasterIK(),
                pointAimRot);
        }

        private void ApplyAbsAim(Vector3 loc, Quaternion rot)
        {
            Vector3 offset = -loc;
            
            GetMasterIK().position = aimTarget.position;
            GetMasterIK().rotation = GetRootBone().rotation * rot;
            CoreToolkitLib.MoveInBoneSpace(GetMasterIK(),GetMasterIK(), -offset);
        }
    }
}