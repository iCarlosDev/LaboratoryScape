using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace IE.RichFX
{

[Serializable, VolumeComponentMenu("Rich FX/Blurs/Simple Gaussian Blur")]
public sealed class SimpleGaussianBlur : CustomPostProcessVolumeComponent, IPostProcessComponent
{

    public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0.0f, 1.0f);
    Material m_Material;

    public bool IsActive() => m_Material != null && intensity.value > 0f;

    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

    public override void Setup()
    {
        if (Shader.Find("Hidden/InanEvin/RichFX/SimpleGaussianBlur") != null)
            m_Material = new Material(Shader.Find("Hidden/InanEvin/RichFX/SimpleGaussianBlur"));
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (m_Material == null)
            return;

        m_Material.SetFloat("_Intensity", intensity.value * 10.0f);
        m_Material.SetTexture("_InputTexture", source);
        HDUtils.DrawFullScreen(cmd, m_Material, destination);
    }

    public override void Cleanup()
    {
        CoreUtils.Destroy(m_Material);
    }
}


}
