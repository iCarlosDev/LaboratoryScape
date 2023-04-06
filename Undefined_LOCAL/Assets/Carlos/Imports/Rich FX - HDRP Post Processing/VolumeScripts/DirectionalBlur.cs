using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace IE.RichFX
{

    [Serializable, VolumeComponentMenu("Rich FX/Blurs/Directional Blur")]
    public sealed class DirectionalBlur : CustomPostProcessVolumeComponent, IPostProcessComponent
    {

        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0.0f, 100.0f);
        public ClampedFloatParameter angle = new ClampedFloatParameter(0.0f, 0.0f, 1.0f);
        public ClampedIntParameter sampleCount = new ClampedIntParameter(15, 1, 30);
        Material m_Material;

        public bool IsActive() => m_Material != null && intensity.value > 0f;

        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        public override void Setup()
        {
            if (Shader.Find("Hidden/InanEvin/RichFX/DirectionalBlur") != null)
                m_Material = new Material(Shader.Find("Hidden/InanEvin/RichFX/DirectionalBlur"));
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (m_Material == null)
                return;

            m_Material.SetFloat("_Intensity", intensity.value);
            m_Material.SetFloat("_SampleCount", sampleCount.value);
            m_Material.SetFloat("_DirX", (Mathf.Sin(angle.value * 6.3f) * intensity.value * 2) / sampleCount.value);
            m_Material.SetFloat("_DirY", (Mathf.Cos(angle.value * 6.3f) * intensity.value * 2) / sampleCount.value);
            m_Material.SetTexture("_InputTexture", source);
            HDUtils.DrawFullScreen(cmd, m_Material, destination);
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(m_Material);
        }
    }

}
