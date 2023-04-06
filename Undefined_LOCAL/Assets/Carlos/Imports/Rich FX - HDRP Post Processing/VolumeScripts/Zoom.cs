using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace IE.RichFX
{


    [Serializable, VolumeComponentMenu("Rich FX/Others/Zoom")]
    public sealed class Zoom : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter scale = new ClampedFloatParameter(0f, 0f, 1f);
        public ClampedFloatParameter centerX = new ClampedFloatParameter(0.5f, 0.0f, 1.0f);
        public ClampedFloatParameter centerY = new ClampedFloatParameter(0.5f, 0.0f, 1.0f);
        Material m_Material;

        public bool IsActive() => m_Material != null && scale.value > 0f;

        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        public override void Setup()
        {
            if (Shader.Find("Hidden/InanEvin/RichFX/Zoom") != null)
                m_Material = new Material(Shader.Find("Hidden/InanEvin/RichFX/Zoom"));
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (m_Material == null)
                return;

            m_Material.SetFloat("_Scale", scale.value);
            m_Material.SetFloat("_CenterX", centerX.value);
            m_Material.SetFloat("_CenterY", centerY.value);
            m_Material.SetTexture("_InputTexture", source);
            HDUtils.DrawFullScreen(cmd, m_Material, destination);
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(m_Material);
        }
    }
}