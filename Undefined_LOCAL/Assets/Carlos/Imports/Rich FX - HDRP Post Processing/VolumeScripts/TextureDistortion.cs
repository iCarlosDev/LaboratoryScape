using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace IE.RichFX
{

    [Serializable, VolumeComponentMenu("Rich FX/Screen Distortions/Texture Distortion")]
    public sealed class TextureDistortion : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0.0f, 0.3f);
        public ClampedFloatParameter speed = new ClampedFloatParameter(0f, 0.0f, 1.0f);
        public TextureParameter distortionTexture = new TextureParameter(null);
        Material m_Material;

        public bool IsActive() => m_Material != null && intensity.value > 0f  && speed.value > 0f && distortionTexture != null;

        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        public override void Setup()
        {
            if (Shader.Find("Hidden/InanEvin/RichFX/TextureDistortion") != null)
                m_Material = new Material(Shader.Find("Hidden/InanEvin/RichFX/TextureDistortion"));
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (m_Material == null)
                return;

            m_Material.SetVector("_DistortionTextureSize", new Vector2(distortionTexture.value.width, distortionTexture.value.height));
            m_Material.SetTexture("_DistortionTexture", distortionTexture.value);
            m_Material.SetFloat("_Intensity", intensity.value);
            m_Material.SetFloat("_Speed", speed.value * 15.0f);
            m_Material.SetTexture("_InputTexture", source);
            HDUtils.DrawFullScreen(cmd, m_Material, destination);
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(m_Material);
        }
    }

}