﻿#if UNITY_2022_2_OR_NEWER && !SAINTSFIELD_UI_TOOLKIT_DISABLE
using UnityEditor.UIElements;
using UnityEngine.UIElements;
#else
using UnityEditor;
#endif

namespace SaintsField.Editor.Unsaintly.Renderer
{
    public class SerializedFieldRenderer: AbsRenderer
    {
        public SerializedFieldRenderer(UnityEditor.Editor editor, UnsaintlyFieldWithInfo fieldWithInfo, bool tryFixUIToolkit=false) : base(editor, fieldWithInfo, tryFixUIToolkit)
        {
        }

#if UNITY_2022_2_OR_NEWER && !SAINTSFIELD_UI_TOOLKIT_DISABLE

        private PropertyField _result;

        public override VisualElement Render()
        {
            PropertyField result = new PropertyField(serializedObject.FindProperty(fieldWithInfo.fieldInfo.Name));

            // ReSharper disable once InvertIf
            if(TryFixUIToolkit && fieldWithInfo.fieldInfo.GetCustomAttributes(typeof(ISaintsAttribute), true).Length == 0)
            {
                // Debug.Log($"{fieldWithInfo.fieldInfo.Name} {arr.Length}");
                _result = result;
                _result.RegisterCallback<GeometryChangedEvent>(OnGeometryChangedEvent);
            }
            return result;
        }

        private void OnGeometryChangedEvent(GeometryChangedEvent evt)
        {
            // Debug.Log("OnGeometryChangedEvent");
            Label label = _result.Q<Label>(className: "unity-label");
            if (label == null)
            {
                return;
            }

            // Utils.Util.FixLabelWidthLoopUIToolkit(label);
            _result.UnregisterCallback<GeometryChangedEvent>(OnGeometryChangedEvent);
            Utils.Util.FixLabelWidthLoopUIToolkit(label);
            _result = null;
        }

#else
        public override void Render()
        {
            SerializedProperty property = serializedObject.FindProperty(fieldWithInfo.fieldInfo.Name);
            EditorGUILayout.PropertyField(property);
        }
#endif
    }
}
