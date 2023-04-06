using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace IE.RichFX
{

    [Serializable, VolumeComponentMenu("Rich FX/Screen Distortions/Distort")]
    public sealed class Distort : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        public FloatParameter speed = new FloatParameter(0f);
        public ClampedFloatParameter amplitude = new ClampedFloatParameter(0f, 0.0f, 0.1f);
        public FloatParameter fractionX = new FloatParameter(50);
        public FloatParameter fractionY = new FloatParameter(25);
        Material m_Material;

        public bool IsActive() => m_Material != null && amplitude.value > 0f;

        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        public override void Setup()
        {
            if (Shader.Find("Hidden/InanEvin/RichFX/Distort") != null)
                m_Material = new Material(Shader.Find("Hidden/InanEvin/RichFX/Distort"));
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (m_Material == null)
                return;

            m_Material.SetFloat("_Speed", speed.value);
            m_Material.SetFloat("_FractionX", fractionX.value);
            m_Material.SetFloat("_FractionY", fractionY.value);
            m_Material.SetFloat("_Amplitude", amplitude.value);
            m_Material.SetTexture("_InputTexture", source);
            HDUtils.DrawFullScreen(cmd, m_Material, destination);
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(m_Material);
        }
    }

}