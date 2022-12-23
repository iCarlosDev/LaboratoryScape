// Designed by Kinemation, 2022

using System;
using Kinemation.FPSFramework.Runtime.Core;
using UnityEngine;

namespace Kinemation.FPSFramework.Runtime.Layers
{
    [Serializable]
    public struct FreeAimData
    {
        public float scalar;
        public float maxValue;
        public float speed;
    }

    [Serializable]
    public struct MoveSwayData
    {
        public Vector3 maxMoveLocSway;
        public Vector3 maxMoveRotSway;
    }

    public class SwayLayer : AnimLayer
    {
        [Header("Deadzone Rotation")]
        [SerializeField] private Transform headBone;
        [SerializeField] private FreeAimData freeAimData;
        [SerializeField] private bool bFreeAim;
        [SerializeField] private bool useCircleMethod;
        
        private Vector3 smoothMoveSwayRot;
        private Vector3 smoothMoveSwayLoc;

        private Quaternion deadZoneRot;
        private Vector2 deadZoneRotTarget;
        private float _freeAimAlpha;

        private Vector2 swayTarget;
        private Vector3 swayLoc;
        private Vector3 swayRot;

        public override void OnAnimUpdate()
        {
            var master = GetMasterIK();
            LocRot baseT = new LocRot(master.position, master.rotation);

            freeAimData = GetGunData().freeAimData;

            ApplySway();
            ApplyFreeAim();
            ApplyMoveSway();

            LocRot newT = new LocRot(GetMasterIK().position, GetMasterIK().rotation);
        
            GetMasterIK().position = Vector3.Lerp(baseT.position, newT.position, layerAlpha);
            GetMasterIK().rotation = Quaternion.Slerp(baseT.rotation, newT.rotation, layerAlpha);
        }

        private void ApplyFreeAim()
        {
            float deltaRight = GetCharData().deltaAimInput.x;
            float deltaUp = GetCharData().deltaAimInput.y;

            if (bFreeAim)
            {
                deadZoneRotTarget.x += deltaUp * freeAimData.scalar;
                deadZoneRotTarget.y += deltaRight * freeAimData.scalar;
            }
            else
            {
                deadZoneRotTarget = Vector2.zero;
            }
            
            float finalAlpha = 1f;
            if (GetActionState() == FPSActionState.Aiming)
            {
                finalAlpha = 0f;
                deadZoneRotTarget = Vector2.zero;
            }
            
            deadZoneRotTarget.x = Mathf.Clamp(deadZoneRotTarget.x, -freeAimData.maxValue, freeAimData.maxValue);
            
            if (useCircleMethod)
            {
                var maxY = Mathf.Sqrt(Mathf.Pow(freeAimData.maxValue, 2f) - Mathf.Pow(deadZoneRotTarget.x, 2f));
                deadZoneRotTarget.y = Mathf.Clamp(deadZoneRotTarget.y, -maxY, maxY);
            }
            else
            {
                deadZoneRotTarget.y = Mathf.Clamp(deadZoneRotTarget.y, -freeAimData.maxValue, freeAimData.maxValue);
            }
            
            deadZoneRot.x = CoreToolkitLib.Glerp(deadZoneRot.x, deadZoneRotTarget.x, freeAimData.speed);
            deadZoneRot.y = CoreToolkitLib.Glerp(deadZoneRot.y, deadZoneRotTarget.y, freeAimData.speed);

            Quaternion q = Quaternion.Euler(new Vector3(deadZoneRot.x, deadZoneRot.y, 0f));
            q.Normalize();

            _freeAimAlpha = CoreToolkitLib.Glerp(_freeAimAlpha, finalAlpha, 15f);
            q = Quaternion.Slerp(Quaternion.identity, q, _freeAimAlpha);
            
            CoreToolkitLib.RotateInBoneSpace(GetRootBone().rotation, headBone,q);
        }

        private void ApplySway()
        {
            var masterDynamic = GetMasterIK();
            
            float deltaRight = core.rigData.characterData.deltaAimInput.x;
            float deltaUp = core.rigData.characterData.deltaAimInput.y;

            swayTarget += new Vector2(deltaRight, deltaUp);
            swayTarget.x = CoreToolkitLib.GlerpLayer(swayTarget.x * 0.01f, 0f, 5f);
            swayTarget.y = CoreToolkitLib.GlerpLayer(swayTarget.y * 0.01f, 0f, 5f);

            Vector3 targetLoc = new Vector3(swayTarget.x,swayTarget.y,0f);
            Vector3 targetRot = new Vector3(swayTarget.y, swayTarget.x, swayTarget.x);

            swayLoc = CoreToolkitLib.SpringInterp(swayLoc, targetLoc, ref core.rigData.gunData.springData.loc);
            swayRot = CoreToolkitLib.SpringInterp(swayRot, targetRot, ref core.rigData.gunData.springData.rot);

            var rot = core.rigData.rootBone.rotation;

            CoreToolkitLib.RotateInBoneSpace(rot, masterDynamic, swayRot);
            CoreToolkitLib.MoveInBoneSpace(core.rigData.rootBone, masterDynamic,
                swayLoc);
        }

        private void ApplyMoveSway()
        {
            var moveRotTarget = new Vector3();
            var moveLocTarget = new Vector3();

            var moveSwayData = GetGunData().moveSwayData;
            var moveInput = GetCharData().moveInput;

            moveRotTarget.x = moveInput.y * moveSwayData.maxMoveRotSway.x;
            moveRotTarget.y = moveInput.x * moveSwayData.maxMoveRotSway.y;
            moveRotTarget.z = moveInput.x * moveSwayData.maxMoveRotSway.z;
            
            moveLocTarget.x = moveInput.x * moveSwayData.maxMoveLocSway.x;
            moveLocTarget.y = moveInput.y * moveSwayData.maxMoveLocSway.y;
            moveLocTarget.z = moveInput.y * moveSwayData.maxMoveLocSway.z;

            smoothMoveSwayRot.x = CoreToolkitLib.Glerp(smoothMoveSwayRot.x, moveRotTarget.x, 4.8f);
            smoothMoveSwayRot.y = CoreToolkitLib.Glerp(smoothMoveSwayRot.y, moveRotTarget.y, 4f);
            smoothMoveSwayRot.z = CoreToolkitLib.Glerp(smoothMoveSwayRot.z, moveRotTarget.z, 6f);
            
            smoothMoveSwayLoc.x = CoreToolkitLib.Glerp(smoothMoveSwayLoc.x, moveLocTarget.x, 3.2f);
            smoothMoveSwayLoc.y = CoreToolkitLib.Glerp(smoothMoveSwayLoc.y, moveLocTarget.y, 4f);
            smoothMoveSwayLoc.z = CoreToolkitLib.Glerp(smoothMoveSwayLoc.z, moveLocTarget.z, 3.5f);
            
            CoreToolkitLib.MoveInBoneSpace(core.rigData.rootBone, GetMasterIK(), 
                smoothMoveSwayLoc);
            
            CoreToolkitLib.RotateInBoneSpace(GetMasterIK().rotation, GetMasterIK(), 
                Quaternion.Euler(smoothMoveSwayRot));
        }
    }
}