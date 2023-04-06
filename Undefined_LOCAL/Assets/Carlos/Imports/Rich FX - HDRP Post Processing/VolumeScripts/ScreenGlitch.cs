using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace IE.RichFX
{
    [Serializable, VolumeComponentMenu("Rich FX/Screen Distortions/Screen Glitch")]
    public sealed class ScreenGlitch : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0.0f, 0.0f, 1.0f);
        public FloatParameter speed = new FloatParameter(2.0f);
        public ClampedFloatParameter xDisplacement = new ClampedFloatParameter(0.0f, -1.0f, 1.0f);
        public FloatParameter randomInferencePower = new FloatParameter(0.15f);
        public FloatParameter linePower = new FloatParameter(0.3f);
        public ClampedFloatParameter colorLerp = new ClampedFloatParameter(0.5f, -1.0f, 1.0f);
        Material m_Material;
        public bool IsActive() => m_Material != null && (intensity.value > 0f);

        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        public override void Setup()
        {
            if (Shader.Find("Hidden/InanEvin/RichFX/ScreenGlitch") != null)
                m_Material = new Material(Shader.Find("Hidden/InanEvin/RichFX/ScreenGlitch"));
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (m_Material == null)
                return;

            m_Material.SetFloat("_Speed", speed.value);
            m_Material.SetFloat("_NoiseWaves", intensity.value * 2.0f);
            m_Material.SetFloat("_XDisplacement", xDisplacement.value * 0.01f);
            m_Material.SetFloat("_RandomInferencePower", linePower.value);
            m_Material.SetFloat("_LinePower",randomInferencePower.value);
            m_Material.SetFloat("_ColorLerp", colorLerp.value);
            m_Material.SetTexture("_InputTexture", source);
            HDUtils.DrawFullScreen(cmd, m_Material, destination);
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(m_Material);
        }
    }

}
