using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace IE.RichFX
{

    [Serializable, VolumeComponentMenu("Rich FX/Screen Distortions/RGB Split")]
    public sealed class RGBSplit : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0.0f, 0.0f, 100.0f);
        public FloatParameter angle = new FloatParameter(0.0f);
        Material m_Material;

        public bool IsActive() => m_Material != null && intensity.value > 0f;

        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        public override void Setup()
        {
            if (Shader.Find("Hidden/InanEvin/RichFX/RGBSplit") != null)
                m_Material = new Material(Shader.Find("Hidden/InanEvin/RichFX/RGBSplit"));
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (m_Material == null)
                return;

            m_Material.SetFloat("_Intensity", intensity.value);
            m_Material.SetFloat("_SinAngle", Mathf.Sin(angle.value));
            m_Material.SetFloat("_CosAngle", Mathf.Cos(angle.value));
            m_Material.SetTexture("_InputTexture", source);
            HDUtils.DrawFullScreen(cmd, m_Material, destination,null, 0);
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(m_Material);
        }
    }


}
