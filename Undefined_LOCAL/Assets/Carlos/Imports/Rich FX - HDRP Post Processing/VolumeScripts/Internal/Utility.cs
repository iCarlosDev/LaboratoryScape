
/* 
 * Author: Inan Evin
 * www.inanevin.com
 *
 */

using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace IE.RichFX
{

    public static class ColorPalettes
    {
        public static List<Vector4[]> colorPalettes = new List<Vector4[]> {
            new Vector4[4] { new Vector4(0.612f, 0.725f, 0.08f), new Vector4(0.549f, 0.667f, 0.07f), new Vector4(0.188f, 0.392f, 0.18f), new Vector4(0.063f, 0.247f, 0.06f) },
            new Vector4[4] {Vector4.zero, new Vector4(0.0f, 0.666f, 0.666f), new Vector4(0.666f, 0.0f, 0.666f), new Vector4(0.666f, 0.666f, 0.666f)  },
            new Vector4[4] {Vector4.zero, new Vector4(0.333f, 1.0f, 1.0f), new Vector4(1.0f, 0.333f, 1.0f), Vector4.one },
            new Vector4[4] {Vector4.zero, new Vector4(0.0f, 0.666f, 0.0f), new Vector4(0.0f, 0.666f, 0.0f), new Vector4(0.666f, 0.333f, 0.0f) },
            new Vector4[4] {Vector4.zero, new Vector4(0.333f, 1.0f, 0.333f), new Vector4(1.0f, 0.333f, 0.333f), new Vector4(1.0f, 1.0f, 0.333f) },
            new Vector4[4] {Vector4.zero, new Vector4(0.0f, 0.666f, 0.666f), new Vector4(0.666f, 0.0f, 0.0f), new Vector4(0.666f, 0.666f, 0.666f) },
            new Vector4[4] { Vector4.zero, new Vector4(0.333f, 0.666f, 0.666f), new Vector4(1.0f, 0.333f, 0.333f), Vector4.one},
            new Vector4[4] {Vector4.zero, new Vector4(0.333f, 0.333f, 0.333f), new Vector4(0.666f, 0.666f, 0.666f), Vector4.one },
        };

        public static List<Vector4> colorPalettes2 = new List<Vector4> {
            new Vector4(0.0f, 0.0f, 0.0f),
            new Vector4(0.0f, 0.0f, 0.666f),
            new Vector4(0.0f, 0.666f, 0.0f),
            new Vector4(0.0f, 0.666f, 0.666f),
            new Vector4(0.666f, 0.0f, 0.0f),
            new Vector4(0.666f, 0.0f, 0.0f),
            new Vector4(0.666f, 0.333f, 0.0f),
            new Vector4(0.666f, 0.666f, 0.666f),
            new Vector4(0.333f, 0.333f, 0.333f),
            new Vector4(0.333f, 0.333f, 1.0f),
            new Vector4(0.333f, 1.0f, 0.333f),
            new Vector4(0.333f, 1.0f, 1.0f),
            new Vector4(1.0f, 0.333f, 0.333f),
            new Vector4(1.0f, 0.333f, 1.0f),
            new Vector4(1.0f, 1.0f, 0.333f),
            new Vector4(1.0f, 1.0f, 1.0f),
        };



    }

    [System.Serializable]
    public sealed class GradientParameter : VolumeParameter<Gradient>
    {
        protected override void OnEnable()
        {
            if (value == null) value = GradientUtility.DefaultGradient;
        }
    }

    public static class GradientUtility
    {
        static readonly GradientColorKey[] _defaultColorKeys = new[]
        {
            new GradientColorKey(Color.blue, 0),
            new GradientColorKey(Color.red, 1)
        };

        static readonly GradientAlphaKey[] _defaultAlphaKeys = new[]
        {
            new GradientAlphaKey(1, 0),
            new GradientAlphaKey(1, 1)
        };

        static readonly int[] _colorKeyPropertyIDs = new[]
        {
            Shader.PropertyToID("_ColorKey0"),
            Shader.PropertyToID("_ColorKey1"),
            Shader.PropertyToID("_ColorKey2"),
            Shader.PropertyToID("_ColorKey3"),
            Shader.PropertyToID("_ColorKey4"),
            Shader.PropertyToID("_ColorKey5"),
            Shader.PropertyToID("_ColorKey6"),
            Shader.PropertyToID("_ColorKey7")
        };

        public static Gradient DefaultGradient
        {
            get
            {
                var g = new Gradient();
                g.SetKeys(_defaultColorKeys, _defaultAlphaKeys);
                return g;
            }
        }

        public static int GetColorKeyPropertyID(int index)
        {
            return _colorKeyPropertyIDs[index];
        }

        public static void SetColorKeys(Material material, GradientColorKey[] colorKeys)
        {
            for (var i = 0; i < 8; i++)
                material.SetVector(
                    GetColorKeyPropertyID(i),
                    colorKeys[Mathf.Min(i, colorKeys.Length - 1)].ToVector()
                );
        }
    }

    public static class GradientColorKeyExtension
    {
        public static Vector4 ToVector(this GradientColorKey key)
        {
            var c = key.color.linear;
            return new Vector4(c.r, c.g, c.b, key.time);
        }
    }


}
