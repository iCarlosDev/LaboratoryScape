using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace IE.RichFX
{
    [Serializable, VolumeComponentMenu("Rich FX/Color Effects/CGA Filter")]
    public sealed class CGAFilter : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        public enum ColorPalette { Palette1, Palette2, Palette3, Palette4, Palette5, Palette6, Palette7, Palette8 };
        [Serializable] public sealed class ColorPaletteTypeParam : VolumeParameter<ColorPalette> { }

        public BoolParameter enabled = new BoolParameter(false);
        public ClampedFloatParameter gamma = new ClampedFloatParameter(0.5f, 0.0f, 2.5f);
        public ColorPaletteTypeParam colorPalette = new ColorPaletteTypeParam { value = ColorPalette.Palette1 };
        Material m_Material;

        public bool IsActive() => m_Material != null && enabled.value;
        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        public override void Setup()
        {
            if (Shader.Find("Hidden/InanEvin/RichFX/CGAFilter") != null)
                m_Material = new Material(Shader.Find("Hidden/InanEvin/RichFX/CGAFilter"));
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (m_Material == null)
                return;

            Vector4[] arr = new Vector4[1] { Vector4.zero };

            if (colorPalette.value == ColorPalette.Palette1)
                m_Material.SetVectorArray("_Colors", ColorPalettes.colorPalettes[0]);
            else if (colorPalette.value == ColorPalette.Palette2)
                m_Material.SetVectorArray("_Colors", ColorPalettes.colorPalettes[1]);
            if (colorPalette.value == ColorPalette.Palette3)
                m_Material.SetVectorArray("_Colors", ColorPalettes.colorPalettes[2]);
            if (colorPalette.value == ColorPalette.Palette4)
                m_Material.SetVectorArray("_Colors", ColorPalettes.colorPalettes[3]);
            if (colorPalette.value == ColorPalette.Palette5)
                m_Material.SetVectorArray("_Colors", ColorPalettes.colorPalettes[4]);
            if (colorPalette.value == ColorPalette.Palette6)
                m_Material.SetVectorArray("_Colors", ColorPalettes.colorPalettes[5]);
            if (colorPalette.value == ColorPalette.Palette7)
                m_Material.SetVectorArray("_Colors", ColorPalettes.colorPalettes[6]);
            if (colorPalette.value == ColorPalette.Palette8)
                m_Material.SetVectorArray("_Colors", ColorPalettes.colorPalettes[7]);

            m_Material.SetFloat("_Intensity", 1.0f);
            m_Material.SetFloat("_Gamma", gamma.value);
            m_Material.SetTexture("_InputTexture", source);
            HDUtils.DrawFullScreen(cmd, m_Material, destination);
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(m_Material);
        }
    }

}
