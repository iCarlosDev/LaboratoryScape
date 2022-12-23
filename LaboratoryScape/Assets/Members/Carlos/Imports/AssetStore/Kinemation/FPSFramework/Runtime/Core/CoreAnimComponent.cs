// Designed by Kinemation, 2022

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kinemation.FPSFramework.Runtime.Core
{
    [Serializable]
    public struct AimOffsetBone
    {
        public Transform bone;
        public Vector2 maxAngle;
    }

    [Serializable]
    public struct AimOffset
    {
        public List<AimOffsetBone> bones;
        public int indexOffset;

        [NonSerialized] public List<Vector2> angles;

        public void Init()
        {
            if (angles == null)
            {
                angles = new List<Vector2>();
            }
            else
            {
                angles.Clear();
            }

            bones ??= new List<AimOffsetBone>();

            for (int i = 0; i < bones.Count - indexOffset; i++)
            {
                var bone = bones[i];
                angles.Add(bone.maxAngle);
            }
        }

        public bool IsValid()
        {
            return bones != null && angles != null;
        }

        public bool IsChanged()
        {
            return bones.Count != angles.Count;
        }
    }

    [Serializable]
    public struct DynamicBone
    {
        public Transform target;
        public Transform hintTarget;
        public GameObject obj;

        public void Retarget()
        {
            if (target == null)
            {
                return;
            }

            obj.transform.position = target.position;
            obj.transform.rotation = target.rotation;
        }
    }

    // Used for detecting zero-frames
    [Serializable]
    public struct CachedBones
    {
        public (Vector3, Quaternion) pelvis;
        public (Vector3, Quaternion) rightHand;
        public (Vector3, Quaternion) leftHand;

        public List<Quaternion> lookUp;
    }

    [Serializable]
    public struct DynamicRigData
    {
        public Animator animator;
        public Transform pelvis;
        public DynamicBone masterDynamic;
        public DynamicBone rightHand;
        public DynamicBone leftHand;
        public DynamicBone rightFoot;
        public DynamicBone leftFoot;

        [Tooltip("Used for mesh space calculations")]
        public Transform rootBone;

        public CharAnimData characterData;
        public WeaponAnimData gunData;

        public void Retarget()
        {
            masterDynamic.Retarget();
            rightHand.Retarget();
            leftHand.Retarget();
            rightFoot.Retarget();
            leftFoot.Retarget();
        }
    }
    
    public abstract class AnimLayer : MonoBehaviour
    {
        [Header("Layer Blending")] 
        [SerializeField, Range(0f, 1f)] public float layerAlpha = 1f;
        
        [Header("Misc")]
        public bool runInEditor;
        protected CoreAnimComponent core;
        
        public void OnRetarget(CoreAnimComponent comp)
        {
            core = comp;
        }

        public virtual void OnPreAnimUpdate()
        {
        }
        
        public virtual void OnAnimUpdate()
        {
        }

        public virtual void OnPostIK()
        {
        }

        protected FPSMovementState GetMovementState()
        {
            return core.rigData.characterData.movementState;
        }
        
        protected FPSActionState GetActionState()
        {
            return core.rigData.characterData.actionState;
        }

        protected FPSPoseState GetPoseState()
        {
            return core.rigData.characterData.poseState;
        }

        protected WeaponAnimData GetGunData()
        {
            return core.rigData.gunData;
        }
        
        protected CharAnimData GetCharData()
        {
            return core.rigData.characterData;
        }

        protected Transform GetMasterIK()
        {
            return core.rigData.masterDynamic.obj.transform;
        }
        
        protected Transform GetRootBone()
        {
            return core.rigData.rootBone;
        }

        protected Animator GetAnimator()
        {
            return core.rigData.animator;
        }
    }
    
    [ExecuteAlways]
    public class CoreAnimComponent : MonoBehaviour
    {
        [Header("Essentials")] 
        public DynamicRigData rigData;
        
        [SerializeField] private List<AnimLayer> animLayers;
        [SerializeField] private bool useIK = true;

        [Header("Misc")] 
        [SerializeField] private bool drawDebug;

        private bool _updateInEditor;
        private float _interpHands;
        private float _interpLayer;

        private void ApplyIK()
        {
            if (!useIK)
            {
                return;
            }
            
            void SolveIK(DynamicBone tipBone)
            {
                var lowerBone = tipBone.target.parent;
                CoreToolkitLib.SolveTwoBoneIK(lowerBone.parent, lowerBone, tipBone.target,
                    tipBone.obj.transform, tipBone.hintTarget, 1f, 1f, 1f);
            }
            
            SolveIK(rigData.rightHand);
            SolveIK(rigData.leftHand);
            SolveIK(rigData.rightFoot);
            SolveIK(rigData.leftFoot);
        }

        private void Update()
        {
            if (!Application.isPlaying && _updateInEditor)
            {
                if (rigData.animator == null)
                {
                    return;
                }

                rigData.animator.Update(Time.deltaTime);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (drawDebug)
            {
                Gizmos.color = Color.green;

                void DrawDynamicBone(ref DynamicBone bone, string boneName)
                {
                    if (bone.obj != null)
                    {
                        var loc = bone.obj.transform.position;
                        Gizmos.DrawWireSphere(loc, 0.06f);
                        Handles.Label(loc, boneName);
                    }
                }
                
                DrawDynamicBone(ref rigData.rightHand, "RightHandIK");
                DrawDynamicBone(ref rigData.leftHand, "LeftHandIK");
                DrawDynamicBone(ref rigData.rightFoot, "RightFootIK");
                DrawDynamicBone(ref rigData.leftFoot, "LeftFootIK");

                Gizmos.color = Color.blue;
                if (rigData.rootBone != null)
                {
                    var mainBone = rigData.rootBone.position;
                    Gizmos.DrawWireCube(mainBone, new Vector3(0.1f, 0.1f, 0.1f));
                    Handles.Label(mainBone, "rootBone");
                }
            }

            if (!Application.isPlaying)
            {
                EditorApplication.QueuePlayerLoopUpdate();
                SceneView.RepaintAll();
            }
        }
#endif

        private void LateUpdate()
        {
            if (!Application.isPlaying && !_updateInEditor)
            {
                return;
            }
            
            Retarget();
            PreUpdateLayers();
            UpdateLayers();
            ApplyIK();
            PostUpdateLayers();
        }
        
        private void Retarget()
        {
            foreach (var layer in animLayers)
            {
                if (!Application.isPlaying && !layer.runInEditor)
                {
                    continue;
                }
                
                layer.OnRetarget(this);
            }
            
            rigData.Retarget();
        }

        // Called right after retargeting
        private void PreUpdateLayers()
        {
            foreach (var layer in animLayers)
            {
                if (!Application.isPlaying && !layer.runInEditor)
                {
                    continue;
                }
                
                layer.OnPreAnimUpdate();
            }
        }

        private void UpdateLayers()
        {
            foreach (var layer in animLayers)
            {
                if (!Application.isPlaying && !layer.runInEditor)
                {
                    continue;
                }
                
                layer.OnAnimUpdate();
            }
        }

        // Called after IK update
        private void PostUpdateLayers()
        {
            foreach (var layer in animLayers)
            {
                if (!Application.isPlaying && !layer.runInEditor)
                {
                    continue;
                }
                
                layer.OnPostIK();
            }
        }

        public void EnableEditorPreview()
        {
            if (rigData.animator == null)
            {
                rigData.animator = GetComponent<Animator>();
            }

            _updateInEditor = true;
        }

        public void DisableEditorPreview()
        {
            _updateInEditor = false;

            if (rigData.animator == null)
            {
                return;
            }

            rigData.animator.Rebind();
            rigData.animator.Update(0f);
        }
        
        public void SetupBones()
        {
            if (rigData.animator == null)
            {
                rigData.animator = GetComponent<Animator>();
            }
            
            if (rigData.rootBone == null)
            {
                var root = transform.Find("rootBone");

                if (root != null)
                {
                    rigData.rootBone = root.transform;
                }
                else
                {
                    var bone = new GameObject("rootBone");
                    bone.transform.parent = transform;
                    rigData.rootBone = bone.transform;
                    rigData.rootBone.localPosition = Vector3.zero;
                }
            }

            if (rigData.rightFoot.obj == null)
            {
                var bone = transform.Find("RightFootIK");

                if (bone != null)
                {
                    rigData.rightFoot.obj = bone.gameObject;
                }
                else
                {
                    rigData.rightFoot.obj = new GameObject("RightFootIK");
                    rigData.rightFoot.obj.transform.parent = transform;
                    rigData.rightFoot.obj.transform.localPosition = Vector3.zero;
                }
            }

            if (rigData.leftFoot.obj == null)
            {
                var bone = transform.Find("LeftFootIK");

                if (bone != null)
                {
                    rigData.leftFoot.obj = bone.gameObject;
                }
                else
                {
                    rigData.leftFoot.obj = new GameObject("LeftFootIK");
                    rigData.leftFoot.obj.transform.parent = transform;
                    rigData.leftFoot.obj.transform.localPosition = Vector3.zero;
                }
            }

            var children = transform.GetComponentsInChildren<Transform>(true);

            bool foundRightHand = false;
            bool foundLeftHand = false;
            bool foundRightFoot = false;
            bool foundLeftFoot = false;
            bool foundHead = false;
            bool foundPelvis = false;

            foreach (var bone in children)
            {
                if (bone.name.ToLower().Contains("ik"))
                {
                    continue;
                }

                bool bMatches = bone.name.ToLower().Contains("hips") || bone.name.ToLower().Contains("pelvis");
                if (!foundPelvis && bMatches)
                {
                    rigData.pelvis = bone;
                    foundPelvis = true;
                    continue;
                }

                bMatches = bone.name.ToLower().Contains("lefthand") || bone.name.ToLower().Contains("hand_l")
                                                                    || bone.name.ToLower().Contains("hand l")
                                                                    || bone.name.ToLower().Contains("l hand")
                                                                    || bone.name.ToLower().Contains("l.hand")
                                                                    || bone.name.ToLower().Contains("hand.l");
                if (!foundLeftHand && bMatches)
                {
                    rigData.leftHand.target = bone;

                    if (rigData.leftHand.hintTarget == null)
                    {
                        rigData.leftHand.hintTarget = bone.parent;
                    }

                    foundLeftHand = true;
                    continue;
                }

                bMatches = bone.name.ToLower().Contains("righthand") || bone.name.ToLower().Contains("hand_r")
                                                                     || bone.name.ToLower().Contains("hand r")
                                                                     || bone.name.ToLower().Contains("r hand")
                                                                     || bone.name.ToLower().Contains("r.hand")
                                                                     || bone.name.ToLower().Contains("hand.r");
                if (!foundRightHand && bMatches)
                {
                    rigData.rightHand.target = bone;

                    if (rigData.rightHand.hintTarget == null)
                    {
                        rigData.rightHand.hintTarget = bone.parent;
                    }

                    foundRightHand = true;
                }

                bMatches = bone.name.ToLower().Contains("rightfoot") || bone.name.ToLower().Contains("foot_r")
                                                                     || bone.name.ToLower().Contains("foot r")
                                                                     || bone.name.ToLower().Contains("r foot")
                                                                     || bone.name.ToLower().Contains("r.foot")
                                                                     || bone.name.ToLower().Contains("foot.r");
                if (!foundRightFoot && bMatches)
                {
                    rigData.rightFoot.target = bone;
                    rigData.rightFoot.hintTarget = bone.parent;

                    foundRightFoot = true;
                }

                bMatches = bone.name.ToLower().Contains("leftfoot") || bone.name.ToLower().Contains("foot_l")
                                                                    || bone.name.ToLower().Contains("foot l")
                                                                    || bone.name.ToLower().Contains("l foot")
                                                                    || bone.name.ToLower().Contains("l.foot")
                                                                    || bone.name.ToLower().Contains("foot.l");
                if (!foundLeftFoot && bMatches)
                {
                    rigData.leftFoot.target = bone;
                    rigData.leftFoot.hintTarget = bone.parent;

                    foundLeftFoot = true;
                }

                if (!foundHead && bone.name.ToLower().Contains("head"))
                {
                    if (rigData.masterDynamic.obj == null)
                    {
                        var boneObject = bone.transform.Find("MasterIK");

                        if (boneObject != null)
                        {
                            rigData.masterDynamic.obj = boneObject.gameObject;
                        }
                        else
                        {
                            rigData.masterDynamic.obj = new GameObject("MasterIK");
                            rigData.masterDynamic.obj.transform.parent = bone;
                            rigData.masterDynamic.obj.transform.localPosition = Vector3.zero;
                        }
                    }
                
                    if (rigData.rightHand.obj == null)
                    {
                        var boneObject = bone.transform.Find("RightHandIK");

                        if (boneObject != null)
                        {
                            rigData.rightHand.obj = boneObject.gameObject;
                        }
                        else
                        {
                            rigData.rightHand.obj = new GameObject("RightHandIK");
                        }

                        rigData.rightHand.obj.transform.parent = rigData.masterDynamic.obj.transform;
                        rigData.rightHand.obj.transform.localPosition = Vector3.zero;
                    }

                    if (rigData.leftHand.obj == null)
                    {
                        var boneObject = bone.transform.Find("LeftHandIK");

                        if (boneObject != null)
                        {
                            rigData.leftHand.obj = boneObject.gameObject;
                        }
                        else
                        {
                            rigData.leftHand.obj = new GameObject("LeftHandIK");
                        }
                        
                        rigData.leftHand.obj.transform.parent = rigData.masterDynamic.obj.transform;
                        rigData.leftHand.obj.transform.localPosition = Vector3.zero;
                    }

                    foundHead = true;
                }
            }

            bool bFound = foundRightHand && foundLeftHand && foundRightFoot && foundLeftFoot && foundHead &&
                          foundPelvis;

            Debug.Log(bFound ? "All bones are found!" : "Some bones are missing!");
        }
        
        public Transform GetRootBone()
        {
            return rigData.rootBone;
        }

        public void OnGunEquipped(WeaponAnimData gunAimData)
        {
            rigData.gunData = gunAimData;
            rigData.masterDynamic.target = rigData.gunData.gunAimData.pivotPoint;
        }

        public void OnSightChanged(Transform newSight)
        {
            rigData.gunData.gunAimData.aimPoint = newSight;
        }

        public void SetMovementState(FPSMovementState state)
        {
            rigData.characterData.movementState = state;
        }

        public void SetActionState(FPSActionState state)
        {
            rigData.characterData.actionState = state;
        }

        public void SetPoseState(FPSPoseState state)
        {
            rigData.characterData.poseState = state;
        }

        public void SetCharData(CharAnimData data)
        {
            rigData.characterData = data;
        }
    }
}