using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace IE.RichFX
{

    [VolumeComponentEditor(typeof(LetterBox))]
    sealed class LetterBoxEditor : VolumeComponentEditor { 

        SerializedDataParameter _aspect;
        SerializedDataParameter _color;
        SerializedDataParameter _enabled;
        SerializedDataParameter _aspectRatioType;


        public override void OnEnable()
        {
            var o = new PropertyFetcher<LetterBox>(serializedObject);
            _color = Unpack(o.Find(x => x.color));
            _aspect = Unpack(o.Find(x => x.customAspect));
            _aspectRatioType = Unpack(o.Find(x => x.aspectRatioType));
            _enabled = Unpack(o.Find(x => x.enabled));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.ApplyModifiedProperties();
            PropertyField(_enabled);
            PropertyField(_color);
            PropertyField(_aspectRatioType);

            var aspectType = (LetterBox.AspectRatioType)_aspectRatioType.value.enumValueIndex;

            if (aspectType == LetterBox.AspectRatioType.Custom)
            {
                PropertyField(_aspect);
            }
         
        }
    }
}