// Designed by Kinemation, 2022

using Kinemation.FPSFramework.Runtime.Core;
using UnityEngine;

namespace Kinemation.FPSFramework.Runtime.Layers
{
    public class SwayLayer : AnimLayer
    {
        [HideInInspector] public Vector2 deltaInput;
        [SerializeField] private LocRotSpringData springData;
        
        private Vector2 swayTarget;
        private Vector3 swayLoc;
        private Vector3 swayRot;
        
        public override void OnAnimUpdate()
        {
            var masterDynamic = rigData.masterDynamic;
            
            float deltaRight = deltaInput.x;
            float deltaUp = deltaInput.y;

            swayTarget += new Vector2(deltaRight, deltaUp);
            swayTarget.x = CoreToolkitLib.GlerpLayer(swayTarget.x * 0.01f, 0f, 5f);
            swayTarget.y = CoreToolkitLib.GlerpLayer(swayTarget.y * 0.01f, 0f, 5f);

            Vector3 targetLoc = new Vector3(swayTarget.x,swayTarget.y,0f);
            Vector3 targetRot = new Vector3(swayTarget.y, swayTarget.x, swayTarget.x);

            swayLoc = CoreToolkitLib.SpringInterp(swayLoc, targetLoc, ref springData.loc);
            swayRot = CoreToolkitLib.SpringInterp(swayRot, targetRot, ref springData.rot);

            var rot = rigData.rootBone.rotation;

            CoreToolkitLib.RotateInBoneSpace(rot, masterDynamic.obj.transform, swayRot);
            CoreToolkitLib.MoveInBoneSpace(rigData.masterDynamic.obj.transform, masterDynamic.obj.transform,
                swayLoc);
        }
        
        public void SetTargetSway(LocRotSpringData swayData)
        {
            springData = swayData;
        }
    }
}
