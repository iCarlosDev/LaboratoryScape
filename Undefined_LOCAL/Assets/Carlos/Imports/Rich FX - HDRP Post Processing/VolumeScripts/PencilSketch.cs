using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace IE.RichFX
{

    [Serializable, VolumeComponentMenu("Rich FX/Artistic/Pencil Sketch")]
    public sealed class PencilSketch : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        public IntParameter steps = new IntParameter(0);
        public ClampedFloatParameter tolerance = new ClampedFloatParameter(0.1f, 0.0f, 1.0f);
        Material m_Material;
        public bool IsActive() => m_Material != null && steps.value > 0f;

        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        public override void Setup()
        {
            if (Shader.Find("Hidden/InanEvin/RichFX/PencilSketch") != null)
                m_Material = new Material(Shader.Find("Hidden/InanEvin/RichFX/PencilSketch"));
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (m_Material == null)
                return;

            m_Material.SetInt("_Steps", steps.value);
            m_Material.SetFloat("_Tolerance", tolerance.value);
            m_Material.SetTexture("_InputTexture", source);
            HDUtils.DrawFullScreen(cmd, m_Material, destination);
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(m_Material);
        }
    }

}