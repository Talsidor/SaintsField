﻿using System;
using System.Linq;
using SaintsField.Editor.Core;
using SaintsField.Editor.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SaintsField.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(GetComponentInChildrenAttribute))]
    public class GetComponentInChildrenAttributeDrawer: SaintsPropertyDrawer
    {
        #region IMGUI
        private string _error = "";

        protected override float GetPostFieldWidth(Rect position, SerializedProperty property, GUIContent label, ISaintsAttribute saintsAttribute) => 0;

        protected override bool DrawPostFieldImGui(Rect position, SerializedProperty property, GUIContent label, ISaintsAttribute saintsAttribute,
            bool valueChanged)
        {
            (string error, UnityEngine.Object result) = DoCheckComponent(property, saintsAttribute);
            if (error != "")
            {
                _error = error;
                return false;
            }
            if(result != null)
            {
                SetValueChanged(property);
            }
            return true;
        }

        protected override bool WillDrawBelow(SerializedProperty property,
            ISaintsAttribute saintsAttribute) => _error != "";

        protected override float GetBelowExtraHeight(SerializedProperty property, GUIContent label, float width, ISaintsAttribute saintsAttribute) => _error == ""? 0: ImGuiHelpBox.GetHeight(_error, width, EMessageType.Error);
        protected override Rect DrawBelow(Rect position, SerializedProperty property, GUIContent label, ISaintsAttribute saintsAttribute) => _error == ""? position: ImGuiHelpBox.Draw(position, _error, EMessageType.Error);
        #endregion

        private static (string error, UnityEngine.Object result) DoCheckComponent(SerializedProperty property, ISaintsAttribute saintsAttribute)
        {
            if (property.objectReferenceValue != null)
            {
                return ("", null);
            }

            GetComponentInChildrenAttribute getComponentInChildrenAttribute = (GetComponentInChildrenAttribute) saintsAttribute;
            Type fieldType = SerializedUtils.GetType(property);

            if (getComponentInChildrenAttribute.CompType == typeof(GameObject))
            {
                return ("You can not use GetComponentInChildren with GameObject type", null);
            }

            Type type = getComponentInChildrenAttribute.CompType ?? fieldType;

            Transform transform;
            switch (property.serializedObject.targetObject)
            {
                case Component component:
                    transform = component.transform;
                    break;
                case GameObject gameObject:
                    transform = gameObject.transform;
                    break;
                default:
                    return ("GetComponentInChildrenAttribute can only be used on Component or GameObject", null);
            }

            // var directChildren = transform.Cast<Transform>();

            Component componentInChildren = null;
                // = transform.GetComponentInChildren(type, getComponentInChildrenAttribute.IncludeInactive);
            foreach (Transform directChildTrans in transform.Cast<Transform>())
            {
                componentInChildren = directChildTrans.GetComponentInChildren(type, getComponentInChildrenAttribute.IncludeInactive);
                if (componentInChildren != null)
                {
                    break;
                }
            }

            if (componentInChildren == null)
            {
                return ($"No {type} found in children", null);
            }

            UnityEngine.Object result = componentInChildren;

            if (fieldType != type)
            {
                if(fieldType == typeof(GameObject))
                {
                    result = componentInChildren.gameObject;
                }
                else
                {
                    result = componentInChildren.GetComponent(fieldType);
                }
            }

            property.objectReferenceValue = result;
            return ("", result);
        }

        #region UIToolkit


        private static string NamePlaceholder(SerializedProperty property, int index) =>
            $"{property.propertyPath}_{index}__GetComponentInChildren";

        protected override VisualElement CreatePostFieldUIToolkit(SerializedProperty property,
            ISaintsAttribute saintsAttribute, int index, VisualElement container, object parent,
            Action<object> onChange)
        {
#if SAINTSFIELD_DEBUG && SAINTSFIELD_DEBUG_DRAW_PROCESS_GET_COMPONENT_IN_CHILDREN
            Debug.Log($"GetComponent DrawPostFieldUIToolkit for {property.propertyPath}");
#endif
            (string error, UnityEngine.Object result) = DoCheckComponent(property, saintsAttribute);
            if (error != "")
            {
                return new VisualElement
                {
                    style =
                    {
                        width = 0,
                    },
                    name = NamePlaceholder(property, index),
                    userData = error,
                };
            }

            property.serializedObject.ApplyModifiedProperties();

            onChange?.Invoke(result);

            return new VisualElement
            {
                style =
                {
                    width = 0,
                },
                name = NamePlaceholder(property, index),
                userData = "",
            };
        }

        // NOTE: ensure the post field is added to the container!
        protected override VisualElement CreateBelowUIToolkit(SerializedProperty property,
            ISaintsAttribute saintsAttribute, int index, VisualElement container, object parent)
        {
            string error = (string)(container.Q<VisualElement>(NamePlaceholder(property, index))!.userData ?? "");
#if SAINTSFIELD_DEBUG && SAINTSFIELD_DEBUG_DRAW_PROCESS_GET_COMPONENT_IN_CHILDREN
            Debug.Log($"GetComponentInChildren error {error}");
#endif
            return string.IsNullOrEmpty(error)
                ? null
                : new HelpBox(_error, HelpBoxMessageType.Error);
        }

        #endregion
    }
}
