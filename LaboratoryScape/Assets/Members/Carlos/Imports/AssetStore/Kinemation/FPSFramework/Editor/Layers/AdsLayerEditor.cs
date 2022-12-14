// Designed by Kinemation, 2022

using Kinemation.FPSFramework.Runtime.Layers;
using UnityEditor;
using UnityEngine;

namespace Kinemation.FPSFramework.Editor.Layers
{
    [CustomEditor(typeof(AdsLayer))]
    public class AdsLayerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var layer = (AdsLayer) target;
            
            if (GUILayout.Button("Calculate Aim Data"))
            {
                layer.CalculateAimData();
            }
        }
    }
}