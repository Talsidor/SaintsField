﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SaintsField.Editor.Core;
using SaintsField.Editor.Utils;
using UnityEditor;
using UnityEngine;
#if UNITY_2021_3_OR_NEWER
using UnityEngine.UIElements;
#endif
using Object = UnityEngine.Object;

namespace SaintsField.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(GetScriptableObjectAttribute))]
    public class GetScriptableObjectAttributeDrawer: SaintsPropertyDrawer
    {
        #region IMGUI
        private string _error = "";

        protected override float GetPostFieldWidth(Rect position, SerializedProperty property, GUIContent label,
            ISaintsAttribute saintsAttribute, FieldInfo info, object parent) => 0;

        protected override bool DrawPostFieldImGui(Rect position, SerializedProperty property, GUIContent label,
            ISaintsAttribute saintsAttribute,
            int index,
            OnGUIPayload onGUIPayload, FieldInfo info, object parent)
        {
            (string error, Object result) = DoCheckComponent(property, saintsAttribute, info);
            if (error != "")
            {
                _error = error;
                return false;
            }
            if(result != null)
            {
                onGUIPayload.SetValue(result);
            }
            return true;
        }

        protected override bool WillDrawBelow(SerializedProperty property,
            ISaintsAttribute saintsAttribute, FieldInfo info, object parent) => _error != "";

        protected override float GetBelowExtraHeight(SerializedProperty property, GUIContent label, float width,
            ISaintsAttribute saintsAttribute, FieldInfo info, object parent) => _error == ""? 0: ImGuiHelpBox.GetHeight(_error, width, EMessageType.Error);
        protected override Rect DrawBelow(Rect position, SerializedProperty property, GUIContent label,
            ISaintsAttribute saintsAttribute, FieldInfo info, object parent) => _error == ""? position: ImGuiHelpBox.Draw(position, _error, EMessageType.Error);

        #endregion

        private static (string error, Object result) DoCheckComponent(SerializedProperty property, ISaintsAttribute saintsAttribute, FieldInfo info)
        {
            if (property.objectReferenceValue != null)
            {
                return ("", null);
            }

            GetScriptableObjectAttribute getScriptableObjectAttribute = (GetScriptableObjectAttribute) saintsAttribute;

            Type fieldType = info.FieldType;
            string nameNoArray = fieldType.Name;
            if(SerializedUtils.PropertyPathIndex(property.propertyPath) != -1)
            {
                fieldType = fieldType.GetElementType();
                Debug.Assert(fieldType != null);
                nameNoArray = fieldType.Name.Replace("[]", "");
            }

            IEnumerable<string> paths = AssetDatabase.FindAssets($"t:{nameNoArray}")
                .Select(AssetDatabase.GUIDToAssetPath);

            if (getScriptableObjectAttribute.PathSuffix != null)
            {
                paths = paths.Where(each => each.EndsWith(getScriptableObjectAttribute.PathSuffix));
            }
            Object result = paths
                .Select(each => AssetDatabase.LoadAssetAtPath(each, fieldType))
                .FirstOrDefault(each => each != null);

            if (result == null)
            {
                return ($"Can not find {nameNoArray} type asset", null);
            }

            property.objectReferenceValue = result;
            return ("", result);
        }

#if UNITY_2021_3_OR_NEWER

        #region UIToolkit

        private static string NamePlaceholder(SerializedProperty property, int index) =>
            $"{property.propertyPath}_{index}__GetScriptableObject";

        protected override void OnAwakeUIToolkit(SerializedProperty property, ISaintsAttribute saintsAttribute,
            int index,
            VisualElement container, Action<object> onValueChangedCallback, FieldInfo info, object parent)
        {
#if SAINTSFIELD_DEBUG && SAINTSFIELD_DEBUG_DRAW_PROCESS_GET_SCRIPTABLE_OBJECT
            Debug.Log($"GetScriptableObject DrawPostFieldUIToolkit for {property.propertyPath}");
#endif
            (string error, Object result) = DoCheckComponent(property, saintsAttribute, info);
            HelpBox helpBox = container.Q<HelpBox>(NamePlaceholder(property, index));
            if (error != helpBox.text)
            {
                helpBox.style.display = error == "" ? DisplayStyle.None : DisplayStyle.Flex;
                helpBox.text = error;
            }

            // ReSharper disable once InvertIf
            if (result)
            {
                property.serializedObject.ApplyModifiedProperties();
                onValueChangedCallback.Invoke(result);
            }
        }

        // NOTE: ensure the post field is added to the container!
        protected override VisualElement CreateBelowUIToolkit(SerializedProperty property,
            ISaintsAttribute saintsAttribute, int index, VisualElement container, FieldInfo info, object parent)
        {
            HelpBox helpBox = new HelpBox("", HelpBoxMessageType.Error)
            {
                style =
                {
                    display = DisplayStyle.None,
                },
                name = NamePlaceholder(property, index),
            };

            helpBox.AddToClassList(ClassAllowDisable);
            return helpBox;
        }
        #endregion

#endif
    }
}
