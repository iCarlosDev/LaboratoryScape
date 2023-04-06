using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace IE.RichFX
{

    [Serializable, VolumeComponentMenu("Rich FX/Others/Letter Box")]
    public sealed class LetterBox : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        public enum AspectRatioType { OneOne, FiveFour, FourThree, ThreeTwo, SixteenNine, SixteenTen, TwentyOneNine, Custom };
        [Serializable] public sealed class AspectRatioParameter : VolumeParameter<AspectRatioType> { }

        public BoolParameter enabled = new BoolParameter(false);
        public ColorParameter color = new ColorParameter(Color.black);
        public AspectRatioParameter aspectRatioType = new AspectRatioParameter { value = AspectRatioType.OneOne };
        public FloatParameter customAspect = new FloatParameter(1.25f);
        Material m_Material;

        public bool IsActive() => m_Material != null && enabled.value;

        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        private float aspect;
        public override void Setup()
        {
            m_Material = CoreUtils.CreateEngineMaterial("Hidden/InanEvin/RichFX/LetterBox");
 
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (m_Material == null)
                return;

            if (aspectRatioType.value == AspectRatioType.Custom)
                aspect = customAspect.value;
            else if (aspectRatioType.value == AspectRatioType.FiveFour)
                aspect = 1.25f;
            if (aspectRatioType.value == AspectRatioType.FourThree)
                aspect = 1.3333f;
            if (aspectRatioType.value == AspectRatioType.OneOne)
                aspect = 1.0f;
            if (aspectRatioType.value == AspectRatioType.SixteenNine)
                aspect = 1.77777f;
            if (aspectRatioType.value == AspectRatioType.SixteenTen)
                aspect = 1.6f;
            if (aspectRatioType.value == AspectRatioType.ThreeTwo)
                aspect = 1.5f;
            if (aspectRatioType.value == AspectRatioType.TwentyOneNine)
                aspect = 2.33333f;


            float w = (float)source.rtHandleProperties.currentViewportSize.x;
            float h = (float)source.rtHandleProperties.currentViewportSize.y;

            float currAspect = w / h;
            float offset = 0.0f;
            int pass = 0;
            
            m_Material.SetColor("_Color", color.value);
            m_Material.SetTexture("_InputTexture", source);

            if (currAspect < aspect - 0.01f)
                offset = (h - w / aspect) * 0.5f / h;
            else if (currAspect > aspect + 0.01f)
            {
                offset = (w - h * aspect) * 0.5f / w;
                pass = 1;
            }
            else
            {
                m_Material.SetFloat("_Offset", 0.0f);
                m_Material.SetFloat("_OffsetInv", 1.0f );
                HDUtils.DrawFullScreen(cmd, m_Material, destination);
                return;
            }
            m_Material.SetFloat("_Offset", offset);
            m_Material.SetFloat("_OffsetInv", 1.0f - offset );
            HDUtils.DrawFullScreen(cmd, m_Material, destination, null, pass);
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(m_Material);
        }
    }

}