using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace IE.RichFX
{
    [Serializable, VolumeComponentMenu("Rich FX/Color Effects/EGA Filter")]
    public sealed class EGAFilter : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter colorDetail = new ClampedFloatParameter(0f, 0f, 5f);
        Material m_Material;

        public bool IsActive() => m_Material != null && colorDetail.value > 0f;

        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        public override void Setup()
        {
            if (Shader.Find("Hidden/InanEvin/RichFX/EGAFilter") != null)
                m_Material = new Material(Shader.Find("Hidden/InanEvin/RichFX/EGAFilter"));
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (m_Material == null)
                return;

            m_Material.SetVectorArray("_Colors", ColorPalettes.colorPalettes2);
            m_Material.SetFloat("_ColorDetail", colorDetail.value);
            m_Material.SetTexture("_InputTexture", source);
            HDUtils.DrawFullScreen(cmd, m_Material, destination);
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(m_Material);
        }
    }

}
