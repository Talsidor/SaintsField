﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SaintsField.DropdownBase;
using SaintsField.Editor.Core;
using SaintsField.Editor.Utils;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityAdvancedDropdown = UnityEditor.IMGUI.Controls.AdvancedDropdown;
using UnityAdvancedDropdownItem = UnityEditor.IMGUI.Controls.AdvancedDropdownItem;

namespace SaintsField.Editor.Drawers
{

    public class SaintsAdvancedDropdown : UnityAdvancedDropdown
    {
        private readonly IAdvancedDropdownList _dropdownListValue;

        private readonly Dictionary<UnityAdvancedDropdownItem, object> _itemToValue = new Dictionary<UnityAdvancedDropdownItem, object>();
        private readonly Action<object> _setValueCallback;

        public SaintsAdvancedDropdown(IAdvancedDropdownList dropdownListValue, AdvancedDropdownState state, Action<object> setValueCallback) : base(state)
        {
            _dropdownListValue = dropdownListValue;
            _setValueCallback = setValueCallback;
        }

        protected override UnityAdvancedDropdownItem BuildRoot()
        {
            AdvancedDropdownItem root = new UnityAdvancedDropdownItem(_dropdownListValue.displayName);

            if(_dropdownListValue.children.Count == 0)
            {
                // root.AddChild(new UnityAdvancedDropdownItem("Empty"));
                return root;
            }

            MakeChildren(root, _dropdownListValue.children);

            return root;
        }

        private void MakeChildren(AdvancedDropdownItem parent, IEnumerable<IAdvancedDropdownList> children)
        {
            // List<(string name, object value, List<object> children, bool disabled, string icon, bool isSeparator)>
            //     childrenCasted =
            //         (List<(string name, object value, List<object> children, bool disabled, string icon, bool
            //             isSeparator)>)children;

            foreach (IAdvancedDropdownList childItem in children)
            {
                // (string name, object value, List<object> grandChildren, bool disabled, string icon, bool isSeparator) =
                //     ((string name, object value, List<object> children, bool disabled, string icon, bool isSeparator))childItem;
                if (childItem.isSeparator)
                {
                    parent.AddSeparator();
                }
                else if (childItem.children.Count == 0)
                {
                    // Debug.Log($"{parent.name}/{childItem.displayName}");
                    AdvancedDropdownItem item = new AdvancedDropdownItem(childItem.displayName);
                    _itemToValue[item] = childItem.value;
                    parent.AddChild(item);
                }
                else
                {
                    AdvancedDropdownItem subParent = new AdvancedDropdownItem(childItem.displayName);
                    // Debug.Log($"{parent.name}/{childItem.displayName}[...]");
                    MakeChildren(subParent, childItem.children);
                    parent.AddChild(subParent);
                }
            }
        }

        protected override void ItemSelected(UnityAdvancedDropdownItem item)
        {
            // Debug.Log($"select {item.name}: {(_itemToValue.TryGetValue(item, out object result) ? result.ToString() : "[NULL]")}");
            if (_itemToValue.TryGetValue(item, out object result))
            {
                _setValueCallback(result);
            }
        }
    }


    [CustomPropertyDrawer(typeof(AdvancedDropdownAttribute))]
    public class AdvancedDropdownAttributeDrawer: SaintsPropertyDrawer
    {
        private string _error = "";

        protected override float GetFieldHeight(SerializedProperty property, GUIContent label, ISaintsAttribute saintsAttribute,
            bool hasLabelWidth)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        private static IEnumerable<(string, object)> FlattenChild(string prefix, IEnumerable<IAdvancedDropdownList> children)
        {
            foreach (IAdvancedDropdownList child in children)
            {
                if (child.children.Count > 0)
                {
                    // List<(string, object, List<object>, bool, string, bool)> grandChildren = child.Item3.Cast<(string, object, List<object>, bool, string, bool)>().ToList();
                    foreach ((string, object) grandChild in FlattenChild(prefix, child.children))
                    {
                        yield return grandChild;
                    }
                }
                else
                {
                    yield return (Prefix(prefix, child.displayName), child.value);
                }
            }
        }

        private static IEnumerable<(string, object)> Flatten(string prefix, IAdvancedDropdownList roots)
        {
            foreach (IAdvancedDropdownList root in roots)
            {
                if (root.children.Count > 0)
                {
                    // IAdvancedDropdownList children = root.Item3.Cast<(string, object, List<object>, bool, string, bool)>().ToList();
                    foreach ((string, object) child in FlattenChild(Prefix(prefix, root.displayName), root.children))
                    {
                        yield return child;
                    }
                }
                else
                {
                    yield return (Prefix(prefix, root.displayName), root.value);
                }
            }

            // AdvancedDropdownItem<T> result = new AdvancedDropdownItem<T>(root.name, root.Value, root.Icon);
            // foreach (AdvancedDropdownItem<T> child in root.children)
            // {
            //     result.AddChild(flatten(child));
            // }
            //
            // return result;
        }

        private static string Prefix(string prefix, string value) => string.IsNullOrEmpty(prefix)? value : $"{prefix}/{value}";

        protected override void DrawField(Rect position, SerializedProperty property, GUIContent label, ISaintsAttribute saintsAttribute)
        {
            AdvancedDropdownAttribute advancedDropdownAttribute = (AdvancedDropdownAttribute) saintsAttribute;

            // SaintsAdvancedDropdown dropdown = new SaintsAdvancedDropdown(new AdvancedDropdownState());
            // dropdown.Show(position);

            string funcName = advancedDropdownAttribute.FuncName;
            object parentObj = GetParentTarget(property);
            Debug.Assert(parentObj != null);
            Type parentType = parentObj.GetType();
            (ReflectUtils.GetPropType getPropType, object fieldOrMethodInfo) =
                ReflectUtils.GetProp(parentType, funcName);

            #region Get List Items
            IAdvancedDropdownList dropdownListValue;

            switch (getPropType)
            {
                case ReflectUtils.GetPropType.NotFound:
                {
                    _error = $"not found `{funcName}` on target `{parentObj}`";
                    DefaultDrawer(position, property, label);
                }
                    return;
                case ReflectUtils.GetPropType.Property:
                {
                    PropertyInfo foundPropertyInfo = (PropertyInfo)fieldOrMethodInfo;
                    dropdownListValue = foundPropertyInfo.GetValue(parentObj) as IAdvancedDropdownList;
                    if (dropdownListValue == null)
                    {
                        _error = $"dropdownListValue is null from `{funcName}` on target `{parentObj}`";
                        DefaultDrawer(position, property, label);
                        return;
                    }
                }
                    break;
                case ReflectUtils.GetPropType.Field:
                {
                    FieldInfo foundFieldInfo = (FieldInfo)fieldOrMethodInfo;
                    dropdownListValue = foundFieldInfo.GetValue(parentObj) as IAdvancedDropdownList;
                    if (dropdownListValue == null)
                    {
                        _error = $"dropdownListValue is null from `{funcName}` on target `{parentObj}`";
                        DefaultDrawer(position, property, label);
                        return;
                    }
                }
                    break;
                case ReflectUtils.GetPropType.Method:
                {
                    MethodInfo methodInfo = (MethodInfo)fieldOrMethodInfo;
                    ParameterInfo[] methodParams = methodInfo.GetParameters();
                    Debug.Assert(methodParams.All(p => p.IsOptional));

                    _error = "";
                    // IEnumerable<AdvancedDropdownItem<object>> result;
                    try
                    {
                        dropdownListValue =
                            methodInfo.Invoke(parentObj, methodParams.Select(p => p.DefaultValue).ToArray()) as IAdvancedDropdownList;
                        // Debug.Log(rawResult);
                        // Debug.Log(rawResult as IDropdownList);
                        // // Debug.Log(rawResult.GetType());
                        // // Debug.Log(rawResult.GetType().Name);
                        // // Debug.Log(typeof(rawResult));
                        //

                        // Debug.Log($"result: {dropdownListValue}");
                    }
                    catch (TargetInvocationException e)
                    {
                        Debug.Assert(e.InnerException != null);
                        _error = e.InnerException.Message;
                        Debug.LogException(e);
                        return;
                    }
                    catch (Exception e)
                    {
                        _error = e.Message;
                        Debug.LogException(e);
                        return;
                    }

                    if (dropdownListValue == null)
                    {
                        _error = $"dropdownListValue is null from `{funcName}()` on target `{parentObj}`";
                        DefaultDrawer(position, property, label);
                        return;
                    }
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(getPropType), getPropType, null);
            }

            #endregion

            #region Get Cur Value
            const BindingFlags bindAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic |
                                          BindingFlags.Public | BindingFlags.DeclaredOnly;
            // Object target = property.serializedObject.targetObject;
            FieldInfo field = parentType.GetField(property.name, bindAttr);
            Debug.Assert(field != null, $"{property.name}/{parentObj}");
            object curValue = field.GetValue(parentObj);
            // Debug.Log($"get cur value {curValue}, {parentObj}->{field}");
            string curDisplay = "";
            Debug.Assert(dropdownListValue != null);
            foreach ((string, object) itemInfos in Flatten("", dropdownListValue))
            {
                string name = itemInfos.Item1;
                object itemValue = itemInfos.Item2;

                if (curValue == null && itemValue == null)
                {
                    curDisplay = name;
                    break;
                }
                if (curValue is UnityEngine.Object curValueObj
                    && curValueObj == itemValue as UnityEngine.Object)
                {
                    curDisplay = name;
                    break;
                }
                if (itemValue == null)
                {
                    // nothing
                }
                else if (itemValue.Equals(curValue))
                {
                    curDisplay = name;
                    break;
                }
            }
            #endregion

            #region Dropdown

            GUI.SetNextControlName(FieldControlName);
            // ReSharper disable once InvertIf
            if (EditorGUI.DropdownButton(position, new GUIContent(curDisplay), FocusType.Keyboard))
            {
                SaintsAdvancedDropdown dropdown = new SaintsAdvancedDropdown(dropdownListValue, new AdvancedDropdownState(),
                    curItem =>
                    {
                        Util.SetValue(property, curItem, parentObj, parentType, field);
                    });
                dropdown.Show(position);
            }

            #endregion
        }

        protected override bool WillDrawBelow(Rect position, SerializedProperty property, GUIContent label, ISaintsAttribute saintsAttribute) => _error != "";

        protected override float GetBelowExtraHeight(SerializedProperty property, GUIContent label, float width, ISaintsAttribute saintsAttribute) => _error == "" ? 0 : HelpBox.GetHeight(_error, width, MessageType.Error);

        protected override Rect DrawBelow(Rect position, SerializedProperty property, GUIContent label, ISaintsAttribute saintsAttribute) => _error == "" ? position : HelpBox.Draw(position, _error, MessageType.Error);
    }
}