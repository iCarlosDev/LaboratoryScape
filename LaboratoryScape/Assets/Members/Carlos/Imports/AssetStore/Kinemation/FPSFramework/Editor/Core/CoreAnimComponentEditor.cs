// Designed by Kinemation, 2022

using Kinemation.FPSFramework.Runtime.Core;
using UnityEditor;
using UnityEngine;

namespace Kinemation.FPSFramework.Editor.Core
{
    [CustomEditor(typeof(CoreAnimComponent))]
    public class CoreAnimComponentEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var layer = (CoreAnimComponent) target;

            if (GUILayout.Button("Setup bones"))
            {
                layer.SetupBones();
            }

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Preview animation"))
            {
                layer.EnableEditorPreview();
            }

            if (GUILayout.Button("Reset pose"))
            {
                layer.DisableEditorPreview();
            }

            GUILayout.EndHorizontal();
        }
    }
}