// Designed by Kinemation, 2022

using System.Collections.Generic;
using Kinemation.FPSFramework.Runtime.Core;
using UnityEngine;

namespace Kinemation.FPSFramework.Runtime.Layers
{
    public class LookLayer : AnimLayer
    {
        public float test;
        
        [SerializeField] private float layerInterpSpeed;
        [SerializeField, Range(0f, 1f)] private float handsLayerAlpha;
        [SerializeField] private float handsLerpSpeed;

        [SerializeField, Range(0f, 1f)] private float pelvisLayerAlpha = 1f;
        [SerializeField] private float pelvisLerpSpeed;
        private float _interpPelvis;

        [Header("Offsets")] 
        [SerializeField] private Vector3 pelvisOffset;

        [Header("Aim Offset")] [SerializeField]
        private AimOffset lookUpOffset;

        [SerializeField] private AimOffset lookRightOffset;

        [SerializeField] private bool enableAutoDistribution;
        [SerializeField] private bool enableManualSpineControl;

        [SerializeField, Range(-90f, 90f)] private float aimUp;
        [SerializeField, Range(-90f, 90f)] private float aimRight;

        [SerializeField] private float smoothAim;

        [Header("Leaning")]
        [SerializeField] [Range(-1, 1)] private int leanDirection;
        [SerializeField] private float leanAmount = 45f;
        [SerializeField] private float leanSpeed;
        
        [Header("Misc")]
        [SerializeField] protected bool detectZeroFrames = true;
        [SerializeField] protected bool checkZeroFootIK = true;
        [SerializeField] protected bool useRightOffset = true;
        
        private float leanInput;
        
        private float _interpHands;
        [SerializeField] private float _interpLayer;
        private Vector2 _smoothAim;

        // Used to detect zero key frames
        [SerializeField] [HideInInspector] private CachedBones cachedBones;
        [SerializeField] [HideInInspector] private CachedBones cacheRef;
        
        //GETTERS && SETTERS//
        public float AimUp
        {
            get => aimUp;
            set => aimUp = value;
        }
        public float AimRight
        {
            get => aimRight;
            set => aimRight = value;
        }
        public Vector3 PelvisOffset
        {
            get => pelvisOffset;
            set => pelvisOffset = value;
        }

        //////////////////////

        public override void OnPreAnimUpdate()
        {
            if (detectZeroFrames)
            {
                CheckZeroFrames();
            }
        }

        public override void OnAnimUpdate()
        {
            ApplySpineLayer();
        }

        public override void OnPostIK()
        {
            if (detectZeroFrames)
            {
                CacheBones();
            }
        }
        
        public void SetLookWeight(float weight)
        {
            layerAlpha = Mathf.Clamp01(weight);
        }

        public void SetAimRotation(Vector2 aimRot)
        {
            aimUp = Mathf.Clamp(aimRot.y, -90f, 90f);
            aimRight = Mathf.Clamp(aimRot.x, -90f, 90f);
        }

        public void SetLeanInput(int direction)
        {
            leanDirection = direction;
        }

        public void SetPelvisWeight(float weight)
        {
            pelvisLayerAlpha = weight;
        }

        private void Awake()
        {
            lookUpOffset.Init();
            lookRightOffset.Init();
        }

        private void OnValidate()
        {
            if (cachedBones.lookUp == null)
            {
                cachedBones.lookUp ??= new List<Quaternion>();
                cacheRef.lookUp ??= new List<Quaternion>();
            }

            if (!lookUpOffset.IsValid() || lookUpOffset.IsChanged())
            {
                lookUpOffset.Init();

                cachedBones.lookUp.Clear();
                cacheRef.lookUp.Clear();

                for (int i = 0; i < lookUpOffset.bones.Count; i++)
                {
                    cachedBones.lookUp.Add(Quaternion.identity);
                    cacheRef.lookUp.Add(Quaternion.identity);
                }
            }

            if (!lookRightOffset.IsValid() || lookRightOffset.IsChanged())
            {
                lookRightOffset.Init();
            }

            void Distribute(ref AimOffset aimOffset)
            {
                if (enableAutoDistribution)
                {
                    bool enable = false;
                    int divider = 1;
                    float sum = 0f;

                    for (int i = 0; i < aimOffset.bones.Count - aimOffset.indexOffset; i++)
                    {
                        if (enable)
                        {
                            var bone = aimOffset.bones[i];
                            bone.maxAngle.x = (90f - sum) / divider;
                            aimOffset.bones[i] = bone;
                            continue;
                        }

                        if (!Mathf.Approximately(aimOffset.bones[i].maxAngle.x, aimOffset.angles[i].x))
                        {
                            divider = aimOffset.bones.Count - aimOffset.indexOffset - (i + 1);
                            enable = true;
                        }

                        sum += aimOffset.bones[i].maxAngle.x;
                    }
                }

                if (enableAutoDistribution)
                {
                    bool enable = false;
                    int divider = 1;
                    float sum = 0f;

                    for (int i = 0; i < aimOffset.bones.Count - aimOffset.indexOffset; i++)
                    {
                        if (enable)
                        {
                            var bone = aimOffset.bones[i];
                            bone.maxAngle.y = (90f - sum) / divider;
                            aimOffset.bones[i] = bone;
                            continue;
                        }

                        if (!Mathf.Approximately(aimOffset.bones[i].maxAngle.y, aimOffset.angles[i].y))
                        {
                            divider = aimOffset.bones.Count - aimOffset.indexOffset - (i + 1);
                            enable = true;
                        }

                        sum += aimOffset.bones[i].maxAngle.y;
                    }
                }

                for (int i = 0; i < aimOffset.bones.Count - aimOffset.indexOffset; i++)
                {
                    aimOffset.angles[i] = aimOffset.bones[i].maxAngle;
                }
            }

            if (lookUpOffset.bones.Count > 0)
            {
                Distribute(ref lookUpOffset);
            }

            if (lookRightOffset.bones.Count > 0)
            {
                Distribute(ref lookRightOffset);
            }
        }
        
        private void CheckZeroFrames()
        {
            if (cachedBones.pelvis.Item1 == core.rigData.pelvis.localPosition)
            {
                core.rigData.pelvis.localPosition = cacheRef.pelvis.Item1;
            }
            
            if (cachedBones.pelvis.Item2 == core.rigData.pelvis.localRotation)
            {
                core.rigData.pelvis.localRotation = cacheRef.pelvis.Item2;
                
                if (checkZeroFootIK)
                {
                    core.rigData.rightFoot.Retarget();
                    core.rigData.leftFoot.Retarget();
                }
            }

            cacheRef.pelvis.Item2 = core.rigData.pelvis.localRotation;

            bool bZeroSpine = false;
            for (int i = 0; i < cachedBones.lookUp.Count; i++)
            {
                var bone = lookUpOffset.bones[i].bone;
                if (bone == null || bone == core.rigData.pelvis)
                {
                    continue;
                }

                if (cachedBones.lookUp[i] == bone.localRotation)
                {
                    bZeroSpine = true;
                    bone.localRotation = cacheRef.lookUp[i];
                }
            }
            
            if (bZeroSpine)
            {
                core.rigData.masterDynamic.Retarget();
                core.rigData.rightHand.Retarget();
                core.rigData.leftHand.Retarget();
            }
            
            cacheRef.pelvis.Item1 = core.rigData.pelvis.localPosition;

            for (int i = 0; i < lookUpOffset.bones.Count; i++)
            {
                var bone = lookUpOffset.bones[i].bone;
                if (bone == null)
                {
                    continue;
                }
                
                cacheRef.lookUp[i] = bone.localRotation;
            }
        }
        
        private void CacheBones()
        {
            cachedBones.pelvis.Item1 = core.rigData.pelvis.localPosition;
            cachedBones.pelvis.Item2 = core.rigData.pelvis.localRotation;
            
            for (int i = 0; i < lookUpOffset.bones.Count; i++)
            {
                var bone = lookUpOffset.bones[i].bone;
                if (bone == null || bone == core.rigData.pelvis)
                {
                    continue;
                }

                cachedBones.lookUp[i] = bone.localRotation;
            }
        }

        private bool BlendLayers()
        {
            var finalAlpha = layerAlpha;
            if (GetMovementState() == FPSMovementState.Sprinting || GetActionState() == FPSActionState.Ready)
            {
                finalAlpha = test;
            }
            
            _interpLayer = CoreToolkitLib.GlerpLayer(_interpLayer, finalAlpha, layerInterpSpeed);
            return Mathf.Approximately(_interpLayer, 0f);
        }

        private void ApplySpineLayer()
        {
            if (BlendLayers())
            {
                return;
            }
            
            if (!enableManualSpineControl)
            {
                aimUp += GetCharData().deltaAimInput.y;
                aimRight += GetCharData().deltaAimInput.x;

                aimUp = Mathf.Clamp(aimUp, -90f, 90f);
                aimRight = Mathf.Clamp(aimRight, -90f, 90f);
                
                if (lookRightOffset.bones.Count == 0 || !useRightOffset)
                {
                    aimRight = 0f;
                }
                
                leanInput = CoreToolkitLib.Glerp(leanInput, leanAmount * GetCharData().leanDirection, 
                    leanSpeed);
            }
            else
            {
                leanInput = CoreToolkitLib.Glerp(leanInput, leanAmount * leanDirection, leanSpeed);
            }

            var finalPelvisAlpha = pelvisLayerAlpha;
            if (GetPoseState() == FPSPoseState.Crouching)
            {
                finalPelvisAlpha = 0f;
            }

            _interpPelvis = CoreToolkitLib.Glerp(_interpPelvis, finalPelvisAlpha * _interpLayer, 
                pelvisLerpSpeed);
            
            Vector3 pelvisFinal = Vector3.Lerp(Vector3.zero, pelvisOffset, _interpPelvis);
            CoreToolkitLib.MoveInBoneSpace(GetRootBone(), core.rigData.pelvis, pelvisFinal);

            _smoothAim.y = CoreToolkitLib.GlerpLayer(_smoothAim.y, aimUp, smoothAim);
            _smoothAim.x = CoreToolkitLib.GlerpLayer(_smoothAim.x, aimRight, smoothAim);

            foreach (var bone in lookRightOffset.bones)
            {
                if (!Application.isPlaying && bone.bone == null)
                {
                    continue;
                }

                float angleFraction = _smoothAim.x >= 0f ? bone.maxAngle.y : bone.maxAngle.x;
                CoreToolkitLib.RotateInBoneSpace(GetRootBone().rotation, bone.bone,
                    new Vector3(0f, _smoothAim.x * _interpLayer / (90f / angleFraction),0f));
            }
            
            foreach (var bone in lookRightOffset.bones)
            {
                if (!Application.isPlaying && bone.bone == null)
                {
                    continue;
                }

                float angleFraction = bone.maxAngle.x;
                CoreToolkitLib.RotateInBoneSpace(GetRootBone().rotation * Quaternion.Euler(0f, _smoothAim.x, 0f), bone.bone,
                    new Vector3(0f, 0f, leanInput * _interpLayer / (90f / angleFraction)));
            }

            Vector3 rightHandLoc = core.rigData.rightHand.obj.transform.position;
            Quaternion rightHandRot = core.rigData.rightHand.obj.transform.rotation;

            Vector3 leftHandLoc = core.rigData.leftHand.obj.transform.position;
            Quaternion leftHandRot = core.rigData.leftHand.obj.transform.rotation;

            foreach (var bone in lookUpOffset.bones)
            {
                if (!Application.isPlaying && bone.bone == null)
                {
                    continue;
                }

                float angleFraction = _smoothAim.y >= 0f ? bone.maxAngle.y : bone.maxAngle.x;

                CoreToolkitLib.RotateInBoneSpace(GetRootBone().rotation * Quaternion.Euler(0f, _smoothAim.x, 0f),
                    bone.bone,
                    new Vector3(_smoothAim.y * _interpLayer / (90f / angleFraction), 0f, 0f));
            }

            _interpHands = CoreToolkitLib.GlerpLayer(_interpHands, handsLayerAlpha, handsLerpSpeed);

            core.rigData.rightHand.obj.transform.position = Vector3.Lerp(rightHandLoc,
                core.rigData.rightHand.obj.transform.position,
                _interpHands);
            core.rigData.rightHand.obj.transform.rotation = Quaternion.Slerp(rightHandRot,
                core.rigData.rightHand.obj.transform.rotation,
                _interpHands);

            core.rigData.leftHand.obj.transform.position = Vector3.Lerp(leftHandLoc, core.rigData.leftHand.obj.transform.position,
                _interpHands);
            core.rigData.leftHand.obj.transform.rotation = Quaternion.Slerp(leftHandRot,
                core.rigData.leftHand.obj.transform.rotation,
                _interpHands);
        }
    }
}