using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace IE.RichFX
{

    [Serializable, VolumeComponentMenu("Rich FX/Others/Low Light Cam")]
    public sealed class LowLightCam : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        public BoolParameter enabled = new BoolParameter(false);
        public ClampedFloatParameter vignetteIntensity = new ClampedFloatParameter(0.75f, 0f, 1.5f);
        public ClampedFloatParameter noiseIntensity = new ClampedFloatParameter(1f, .1f, 2f);
        public ClampedFloatParameter contrast = new ClampedFloatParameter(1.5f, 1.2f, 1.8f);
        public ClampedFloatParameter brightness = new ClampedFloatParameter(0.7f, 0.6f, 0.9f);
        Material m_Material;

        public bool IsActive() => m_Material != null && enabled.value;

        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        public override void Setup()
        {
            if (Shader.Find("Hidden/InanEvin/RichFX/LowLightCam") != null)
                m_Material = new Material(Shader.Find("Hidden/InanEvin/RichFX/LowLightCam"));
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (m_Material == null)
                return;

            m_Material.SetFloat("_VignetteIntensity", vignetteIntensity.value);
            m_Material.SetFloat("_NoiseIntensity", noiseIntensity.value);
            m_Material.SetFloat("_Intensity", 1.0f);
            m_Material.SetFloat("_Brightness", brightness.value);
            m_Material.SetFloat("_Contrast", contrast.value);
            m_Material.SetTexture("_InputTexture", source);
            HDUtils.DrawFullScreen(cmd, m_Material, destination);
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(m_Material);
        }
    }

}