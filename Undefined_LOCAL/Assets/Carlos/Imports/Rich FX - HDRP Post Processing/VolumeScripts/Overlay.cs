using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using SerializableAttribute = System.SerializableAttribute;


namespace IE.RichFX
{
    [Serializable, VolumeComponentMenu("Rich FX/Color Effects/Overlay")]
    public sealed class Overlay : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        public enum SourceType { Color, Gradient, Texture }
        public enum BlendMode { Normal, Screen, Overlay, Multiply, SoftLight, HardLight }

        [Serializable] public sealed class SourceTypeParameter : VolumeParameter<SourceType> { }
        [Serializable] public sealed class BlendModeParameter : VolumeParameter<BlendMode> { }

        public SourceTypeParameter sourceType = new SourceTypeParameter { value = SourceType.Gradient };
        public BlendModeParameter blendMode = new BlendModeParameter { value = BlendMode.Overlay };
        public ClampedFloatParameter opacity = new ClampedFloatParameter(0, 0, 1);
        public ColorParameter color = new ColorParameter(Color.red, false, false, true);
        public GradientParameter gradient = new GradientParameter();
        public ClampedFloatParameter angle = new ClampedFloatParameter(0, -180, 180);
        public TextureParameter texture = new TextureParameter(null);
        public BoolParameter sourceAlpha = new BoolParameter(true);
        GradientColorKey[] _gradientCache;
        Material m_Material;

        public bool IsActive() => m_Material != null && opacity.value > 0f;

        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

        public override void Setup()
        {
            m_Material = CoreUtils.CreateEngineMaterial("Hidden/InanEvin/RichFX/Overlay");

#if !UNITY_EDITOR
            // At runtime, copy gradient color keys only once on initialization.
            _gradientCache = gradient.value.colorKeys;
#endif
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
 
            m_Material.SetFloat("_Opacity", opacity.value);

            var pass = (int)blendMode.value * 3;

            if (sourceType == Overlay.SourceType.Color)
            {
                // Single color mode parameters
                m_Material.SetColor("_Color", color.value);
                m_Material.SetTexture("_OverlayTexture", Texture2D.whiteTexture);
                m_Material.SetFloat("_UseTextureAlpha", 0);
            }
            else if (sourceType == Overlay.SourceType.Gradient)
            {
#if UNITY_EDITOR
                // In editor, copy gradient color keys every frame.
                _gradientCache = gradient.value.colorKeys;
#endif

                // Gradient direction vector
                var rad = Mathf.Deg2Rad * angle.value;
                var dir = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));

                // Gradient mode parameters
                m_Material.SetVector("_Direction", dir);
                GradientUtility.SetColorKeys(m_Material, _gradientCache);
                pass += _gradientCache.Length > 3 ? 2 : 1;
            }
            else // Overlay.Source.Texture
            {
                // Skip when no texture is given.
                if (texture.value == null) return;

                // Texture mode parameters
                m_Material.SetColor("_Color", Color.white);
                m_Material.SetTexture("_OverlayTexture", texture.value);
                m_Material.SetFloat("_UseTextureAlpha", sourceAlpha.value ? 1 : 0);
            }

            // Blit to destRT with the overlay shader.
            m_Material.SetTexture("_InputTexture", source);
            HDUtils.DrawFullScreen(cmd, m_Material, destination, null, pass);
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(m_Material);
        }
    }
}