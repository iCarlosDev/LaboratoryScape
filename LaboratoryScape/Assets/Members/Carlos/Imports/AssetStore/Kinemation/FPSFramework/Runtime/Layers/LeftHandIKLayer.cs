// Designed by Kinemation, 2022

using Kinemation.FPSFramework.Runtime.Core;
using UnityEngine;

namespace Kinemation.FPSFramework.Runtime.Layers
{
    public class LeftHandIKLayer : AnimLayer
    {
        public Transform leftHandTarget;

        public override void OnAnimUpdate()
        {
            rigData.leftHand.obj.transform.position = leftHandTarget.position;
            rigData.leftHand.obj.transform.rotation = leftHandTarget.rotation;
        }
    }
}