// Designed by Kinemation, 2022

using Kinemation.FPSFramework.Runtime.Core;
using UnityEngine;

namespace Kinemation.FPSFramework.Runtime.Layers
{
    public class BlendingLayer : AnimLayer
    {
        // Source static pose
        [SerializeField] private AnimationClip anim;
        // Character ref
        [SerializeField] private GameObject character;
        [SerializeField] private Transform spineRootBone;
        [SerializeField] private Quaternion spineBoneRotMS;

        private float _smoothAlpha;

        private void Start()
        {
            _smoothAlpha = layerAlpha;
        }

        // MS: mesh space
        public void EvaluateSpineMS()
        {
            if (character == null || anim == null || GetRootBone() == null || spineRootBone == null)
            {
                return;
            }

            Vector3 cachedLoc = character.transform.position;
            anim.SampleAnimation(character, 0f);
            character.transform.position = cachedLoc;

            // To mesh space
            spineBoneRotMS = Quaternion.Inverse(GetRootBone().rotation) * spineRootBone.rotation;
        }

        public override void OnPreAnimUpdate()
        {
            var finalAlpha = layerAlpha;
            if (GetMovementState() == FPSMovementState.Sprinting)
            {
                finalAlpha = 0f;
            }
            
            _smoothAlpha = CoreToolkitLib.Glerp(_smoothAlpha, finalAlpha, 15f);
            spineRootBone.rotation = Quaternion.Slerp(spineRootBone.rotation,
                GetRootBone().rotation * spineBoneRotMS, _smoothAlpha);
        }
    }
}
