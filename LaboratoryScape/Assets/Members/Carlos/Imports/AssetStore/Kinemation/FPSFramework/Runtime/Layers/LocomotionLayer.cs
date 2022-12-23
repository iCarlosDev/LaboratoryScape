// Designed by Kinemation, 2022

using Kinemation.FPSFramework.Runtime.Core;
using UnityEngine;

namespace Kinemation.FPSFramework.Runtime.Layers
{
    public enum ReadyPose
    {
        LowReady,
        HighReady
    }

    public class LocomotionLayer : AnimLayer
    {
        [SerializeField] public LocRot highReadyPose;
        [SerializeField] public LocRot lowReadyPose;
        [SerializeField] private float interpSpeed;

        private float _alpha;
        private float _readyPoseAlpha;
        private float _locoAlpha;

        [SerializeField] private ReadyPose readyPoseType;
        
        private static readonly int RotX = Animator.StringToHash("RotX");
        private static readonly int RotY = Animator.StringToHash("RotY");
        private static readonly int RotZ = Animator.StringToHash("RotZ");
        private static readonly int LocX = Animator.StringToHash("LocX");
        private static readonly int LocY = Animator.StringToHash("LocY");
        private static readonly int LocZ = Animator.StringToHash("LocZ");

        public override void OnAnimUpdate()
        {
            var masterDynamic= GetMasterIK();
            LocRot baseT = new LocRot(masterDynamic.position, masterDynamic.rotation);
            
            _readyPoseAlpha = 0f;
            if (GetActionState() == FPSActionState.Ready)
            {
                _readyPoseAlpha = 1f;
            }

            _alpha = CoreToolkitLib.Glerp(_alpha, _readyPoseAlpha * layerAlpha, interpSpeed);
            
            ApplyReadyPose();
            ApplyLocomotion();
            
            LocRot newT = new LocRot(masterDynamic.position, masterDynamic.rotation);

            masterDynamic.position = Vector3.Lerp(baseT.position, newT.position, layerAlpha);
            masterDynamic.rotation = Quaternion.Slerp(baseT.rotation, newT.rotation, layerAlpha);
        }

        private void ApplyReadyPose()
        {
            var master = GetMasterIK();

            var finalPose = readyPoseType == ReadyPose.HighReady ? highReadyPose : lowReadyPose;
            CoreToolkitLib.MoveInBoneSpace(GetRootBone(), master, 
                Vector3.Lerp(Vector3.zero, finalPose.position, _alpha));
            CoreToolkitLib.RotateInBoneSpace(GetRootBone().rotation, master,
                Quaternion.Slerp(Quaternion.identity, finalPose.rotation, _alpha));
        }

        private void ApplyLocomotion()
        {
            if (GetActionState() == FPSActionState.Aiming || GetActionState() == FPSActionState.PointAiming)
            {
                _locoAlpha = CoreToolkitLib.Glerp(_locoAlpha, 0.5f, 5f);
            }
            else
            {
                _locoAlpha = CoreToolkitLib.Glerp(_locoAlpha, 1f, 5f);
            }

            var master = GetMasterIK();
            var animator = GetAnimator();

            Vector3 curveData = new Vector3();
            curveData.x = animator.GetFloat(RotX);
            curveData.y = animator.GetFloat(RotY);
            curveData.z = animator.GetFloat(RotZ);

            var animRot = Quaternion.Euler(curveData * 100f * layerAlpha);
            animRot.Normalize();
            
            curveData.x = animator.GetFloat(LocX);
            curveData.y = animator.GetFloat(LocY);
            curveData.z = animator.GetFloat(LocZ);
            
            CoreToolkitLib.MoveInBoneSpace(GetRootBone(), master, 
                Vector3.Lerp(Vector3.zero, curveData * layerAlpha / 100f, _locoAlpha));

            CoreToolkitLib.RotateInBoneSpace(GetRootBone().rotation, master,
                Quaternion.Slerp(Quaternion.identity, animRot, _locoAlpha));
        }
    }
}
