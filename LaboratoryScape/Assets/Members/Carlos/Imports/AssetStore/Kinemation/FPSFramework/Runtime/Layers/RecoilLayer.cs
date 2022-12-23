// Designed by Kinemation, 2022

using Kinemation.FPSFramework.Runtime.Core;
using UnityEngine;

namespace Kinemation.FPSFramework.Runtime.Layers
{
    public class RecoilLayer : AnimLayer
    {
        [SerializeField] private bool useMeshSpace;
        
        public override void OnAnimUpdate()
        {
            var masterDynamic = GetMasterIK();
            var recoilAnim = GetCharData().recoilAnim;
            
            LocRot baseT = new LocRot(masterDynamic.position, masterDynamic.rotation);

            if (useMeshSpace)
            {
                CoreToolkitLib.MoveInBoneSpace(GetRootBone(), masterDynamic,
                    recoilAnim.position);

                CoreToolkitLib.RotateInBoneSpace(GetRootBone().rotation, masterDynamic,
                    recoilAnim.rotation);
            }
            else
            {
                CoreToolkitLib.MoveInBoneSpace(masterDynamic, masterDynamic,
                    recoilAnim.position);

                CoreToolkitLib.RotateInBoneSpace(masterDynamic.rotation, masterDynamic,
                    recoilAnim.rotation);
            }
            
            LocRot newT = new LocRot(masterDynamic.position, masterDynamic.rotation);

            masterDynamic.position = Vector3.Lerp(baseT.position, newT.position, layerAlpha);
            masterDynamic.rotation = Quaternion.Slerp(baseT.rotation, newT.rotation, layerAlpha);
        }
    }
}
