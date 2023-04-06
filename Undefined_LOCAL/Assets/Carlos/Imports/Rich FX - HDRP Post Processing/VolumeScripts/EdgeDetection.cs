using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace IE.RichFX
{

    [Serializable, VolumeComponentMenu("Rich FX/Edge Effects/Edge Detection")]
    public sealed class EdgeDetection : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter threshold = new ClampedFloatParameter(0f, 0f, 1f);
        Material m_Material;

        public bool IsActive() => m_Material != null && threshold.value > 0f;

        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        public override void Setup()
        {
            if (Shader.Find("Hidden/InanEvin/RichFX/EdgeDetection") != null)
                m_Material = new Material(Shader.Find("Hidden/InanEvin/RichFX/EdgeDetection"));
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (m_Material == null)
                return;

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