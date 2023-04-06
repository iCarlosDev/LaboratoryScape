using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace IE.RichFX
{


    [Serializable, VolumeComponentMenu("Rich FX/Screen Distortions/Wobble")]
    public sealed class Wobble : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter amplitude = new ClampedFloatParameter(0.0030f, 0f, 0.1f);
        public FloatParameter speed = new FloatParameter(10);
        public FloatParameter frequence = new FloatParameter(0.0f);
        Material m_Material;
        public bool IsActive() => m_Material != null && frequence.value > 0f;

        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        public override void Setup()
        {
            if (Shader.Find("Hidden/InanEvin/RichFX/Wobble") != null)
                m_Material = new Material(Shader.Find("Hidden/InanEvin/RichFX/Wobble"));
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (m_Material == null)
                return;

            m_Material.SetFloat("_Amplitude", amplitude.value);
            m_Material.SetFloat("_Speed", speed.value);
            m_Material.SetFloat("_Frequence", frequence.value);
            m_Material.SetTexture("_InputTexture", source);
            HDUtils.DrawFullScreen(cmd, m_Material, destination);
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(m_Material);
        }
    }
}