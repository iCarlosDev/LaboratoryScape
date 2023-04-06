using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace IE.RichFX
{

    [Serializable, VolumeComponentMenu("Rich FX/Artistic/Posterize")]
    public sealed class Posterize : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        public FloatParameter _Gamma = new FloatParameter(2.0f);
        public ClampedIntParameter _ColorCount = new ClampedIntParameter(0, 0, 1024);
        Material m_Material;
        public bool IsActive() => m_Material != null && _ColorCount.value > 0f;

        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        public override void Setup()
        {
            if (Shader.Find("Hidden/InanEvin/RichFX/Posterize") != null)
                m_Material = new Material(Shader.Find("Hidden/InanEvin/RichFX/Posterize"));
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (m_Material == null)
                return;

            m_Material.SetFloat("_Gamma", _Gamma.value);
            m_Material.SetFloat("_ColorCount", _ColorCount.value);
            m_Material.SetTexture("_InputTexture", source);
            HDUtils.DrawFullScreen(cmd, m_Material, destination);
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(m_Material);
        }
    }

}