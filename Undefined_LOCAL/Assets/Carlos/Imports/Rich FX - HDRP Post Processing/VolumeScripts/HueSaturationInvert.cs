using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace IE.RichFX
{

[Serializable, VolumeComponentMenu("Rich FX/Color Effects/Hue Saturation Invert")]
public sealed class HueSaturationInvert : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    public ClampedFloatParameter hueShift = new ClampedFloatParameter(0, -1, 1);
    public ClampedFloatParameter saturation = new ClampedFloatParameter(1, 0, 2);
    public ClampedFloatParameter invert = new ClampedFloatParameter(0, 0, 1);
    Material m_Material;

    public bool IsActive() => m_Material != null && (hueShift.value != 0f || saturation.value != 1f || invert.value > 0f);

    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

    public override void Setup()
    {
        if (Shader.Find("Hidden/InanEvin/RichFX/HueSaturationInvert") != null)
            m_Material = new Material(Shader.Find("Hidden/InanEvin/RichFX/HueSaturationInvert"));
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (m_Material == null)
            return;

        m_Material.SetFloat("_Hue", hueShift.value);
        m_Material.SetFloat("_Saturation", saturation.value);
        m_Material.SetFloat("_Invert", invert.value);
        m_Material.SetTexture("_InputTexture", source);
        HDUtils.DrawFullScreen(cmd, m_Material, destination);
    }

    public override void Cleanup()
    {
        CoreUtils.Destroy(m_Material);
    }
}

}