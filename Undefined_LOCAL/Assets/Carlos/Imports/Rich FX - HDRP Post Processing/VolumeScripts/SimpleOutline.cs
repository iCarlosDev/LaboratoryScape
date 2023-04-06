using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace IE.RichFX
{
    [Serializable, VolumeComponentMenu("Rich FX/Edge Effects/Simple Outline")]
    public sealed class SimpleOutline : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
        public ClampedFloatParameter threshold = new ClampedFloatParameter(0.01f, 0.01f, 1f);
        Material m_Material;

        public bool IsActive() => m_Material != null && intensity.value > 0f;

        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        public override void Setup()
        {
            if (Shader.Find("Hidden/InanEvin/RichFX/SimpleOutline") != null)
                m_Material = new Material(Shader.Find("Hidden/InanEvin/RichFX/SimpleOutline"));
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (m_Material == null)
                return;

            m_Material.SetFloat("_Intensity", intensity.value * 15.0f);
            m_Material.SetFloat("_Threshold", threshold.value);
            m_Material.SetTexture("_InputTexture", source);
            HDUtils.DrawFullScreen(cmd, m_Material, destination);
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(m_Material);
        }
    }

}
