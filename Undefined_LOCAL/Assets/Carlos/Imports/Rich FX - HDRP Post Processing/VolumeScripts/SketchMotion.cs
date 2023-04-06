using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace IE.RichFX
{
    [Serializable, VolumeComponentMenu("Rich FX/Artistic/Sketch Motion")]
    public sealed class SketchMotion : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter sketchiness = new ClampedFloatParameter(0.0f, 0.0f, 1.0f);
        public ClampedFloatParameter speed = new ClampedFloatParameter(100.0f, 0.0f, 500.0f);
        public ClampedFloatParameter motionAmount =  new ClampedFloatParameter(1, 0.0f, 5.0f);
        public ClampedFloatParameter baseModifier = new ClampedFloatParameter(0.902f, 0.9f, 1.0f);
        public BoolParameter invert = new BoolParameter(true);
        public BoolParameter colored = new BoolParameter(false);
        Material m_Material;
        public bool IsActive() => m_Material != null && sketchiness.value > 0f;

        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        public override void Setup()
        {
            if (Shader.Find("Hidden/InanEvin/RichFX/SketchMotion") != null)
                m_Material = new Material(Shader.Find("Hidden/InanEvin/RichFX/SketchMotion"));
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (m_Material == null)
                return;

            if (invert.value)
                m_Material.EnableKeyword("INVERT");
            else
                m_Material.DisableKeyword("INVERT");

            m_Material.SetFloat("_Sketchiness", sketchiness.value / 100.0f);
            m_Material.SetFloat("_Speed", speed.value);
            m_Material.SetFloat("_MotionAmount", motionAmount.value);
            m_Material.SetFloat("_BaseModifier", baseModifier.value); 
            m_Material.SetInt("_Colored", colored.value ? 1 : 0); 
            m_Material.SetTexture("_InputTexture", source);
            HDUtils.DrawFullScreen(cmd, m_Material, destination);
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(m_Material);
        }
    }

}
