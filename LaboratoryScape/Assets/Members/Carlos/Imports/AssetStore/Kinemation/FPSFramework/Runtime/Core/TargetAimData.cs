// Designed by Kinemation, 2022

using System;
using UnityEngine;

namespace Kinemation.FPSFramework.Runtime.Core
{
    [Serializable]
    public struct GunAimData
    {
        public TargetAimData target;
        public Transform pivotPoint;
        public Transform aimPoint;
        public LocRot pointAimOffset;
        public float aimSpeed;
    }
    
    [CreateAssetMenu(fileName = "NewAimData", menuName = "TargetAimData")]
    public class TargetAimData : ScriptableObject
    {
        public Vector3 aimLoc;
        public Quaternion aimRot;
        public AnimationClip staticPose;
        public string stateName;
    }
}