using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace IE.RichFX
{

    [Serializable, VolumeComponentMenu("Rich FX/Screen Distortions/Displace View")]
    public sealed class DisplaceView : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        public Vector2Parameter amount = new Vector2Parameter(new Vector2(0.0f, 0.0f));
        Material m_Material;

        public bool IsActive() => m_Material != null && (amount.value.magnitude > 0f);

        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        public override void Setup()
        {
            if (Shader.Find("Hidden/InanEvin/RichFX/DisplaceView") != null)
                m_Material = new Material(Shader.Find("Hidden/InanEvin/RichFX/DisplaceView"));
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (m_Material == null)
                return;

            m_Material.SetFloat("_AmountX", amount.value.x);
            m_Material.SetFloat("_AmountY", amount.value.y);
            m_Material.SetTexture("_InputTexture", source);
            HDUtils.DrawFullScreen(cmd, m_Material, destination);
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(m_Material);
        }
    }

}