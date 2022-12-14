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
        [SerializeField] private Transform rootBone;
        [SerializeField] private Transform spineRootBone;
        [SerializeField] private Quaternion spineBoneRotMS;

        [SerializeField] private bool layerEnabled = true;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                layerEnabled = true;
            }
        }

        // MS: mesh space
        public void EvaluateSpineMS()
        {
            if (character == null || anim == null || rootBone == null || spineRootBone == null)
            {
                return;
            }

            Vector3 cachedLoc = character.transform.position;
            anim.SampleAnimation(character, 0f);
            character.transform.position = cachedLoc;

            // To mesh space
            spineBoneRotMS = Quaternion.Inverse(rootBone.rotation) * spineRootBone.rotation;
        }

        public override void OnPreAnimUpdate()
        {
            if (!layerEnabled)
            {
                return;
            }
            spineRootBone.rotation = rootBone.rotation * spineBoneRotMS;
        }
    }
}
