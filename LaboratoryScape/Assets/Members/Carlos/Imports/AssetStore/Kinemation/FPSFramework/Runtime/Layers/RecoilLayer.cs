// Designed by Kinemation, 2022

using Kinemation.FPSFramework.Runtime.Core;

namespace Kinemation.FPSFramework.Runtime.Layers
{
    public class RecoilLayer : AnimLayer
    {
        private LocRot recoilAnim;
        
        public void SetRecoilAnim(LocRot locRot)
        {
            recoilAnim = locRot;
        }

        public override void OnAnimUpdate()
        {
            var masterDynamic = rigData.masterDynamic;
            CoreToolkitLib.MoveInBoneSpace(masterDynamic.obj.transform, masterDynamic.obj.transform,
                recoilAnim.position);

            CoreToolkitLib.RotateInBoneSpace(masterDynamic.obj.transform.rotation, masterDynamic.obj.transform,
                recoilAnim.rotation);
        }
    }
}
