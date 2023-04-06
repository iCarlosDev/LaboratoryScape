using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace IE.RichFX
{

    [Serializable, VolumeComponentMenu("Rich FX/Artistic/Rainbow Flow")]
    public sealed class RainbowFlow : CustomPostProcessVolumeComponent, IPostProcessComponent
    {

        public IntParameter steps = new IntParameter(0);
        public ClampedFloatParameter speed = new ClampedFloatParameter(0.15f, 0.0f, 50.0f);
        public FloatParameter multiplier = new FloatParameter(0.5f);
        public BoolParameter blackAndWhite = new BoolParameter(false);
        Material m_Material;
        public bool IsActive() => m_Material != null && steps.value > 0f;

        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        public override void Setup()
        {
            if (Shader.Find("Hidden/InanEvin/RichFX/RainbowFlow") != null)
                m_Material = new Material(Shader.Find("Hidden/InanEvin/RichFX/RainbowFlow"));
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (m_Material == null)
                return;

            if (blackAndWhite.value)
                m_Material.EnableKeyword("ISBW");
            else
                m_Material.DisableKeyword("ISBW");

            m_Material.SetFloat("_Multiplier", multiplier.value);
            m_Material.SetFloat("_Speed", speed.value);
            m_Material.SetFloat("_Steps", steps.value);
            m_Material.SetTexture("_InputTexture", source);
            HDUtils.DrawFullScreen(cmd, m_Material, destination);
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(m_Material);
        }
    }

}