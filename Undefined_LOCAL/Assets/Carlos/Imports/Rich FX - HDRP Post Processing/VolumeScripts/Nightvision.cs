using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace IE.RichFX
{
    [Serializable, VolumeComponentMenu("Rich FX/Others/Nightvision")]
    public sealed class Nightvision : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        public BoolParameter enabled = new BoolParameter(false);
        public ClampedFloatParameter darkness  = new ClampedFloatParameter(0.5f, 0f, 1f);
        public ClampedFloatParameter greenPower  = new ClampedFloatParameter(0.0f, 0f, 1f);


        Material m_Material;

        public bool IsActive() => m_Material != null && enabled.value;

        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        public override void Setup()
        {
            if (Shader.Find("Hidden/InanEvin/RichFX/Nightvision") != null)
                m_Material = new Material(Shader.Find("Hidden/InanEvin/RichFX/Nightvision"));
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (m_Material == null)
                return;

            m_Material.SetFloat("_Intensity", 1.0f);
            m_Material.SetFloat("_Darkness", darkness.value);
            m_Material.SetFloat("_GreenPower", greenPower.value);
            m_Material.SetTexture("_InputTexture", source);
            HDUtils.DrawFullScreen(cmd, m_Material, destination);
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(m_Material);
        }
    }

}
