﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace SaintsField.Editor.Drawers.VisibilityDrawers
{
    [CustomPropertyDrawer(typeof(HideIfAttribute))]
    public class HideIfAttributeDrawer: ShowIfAttributeDrawer
    {
        protected override (string error, bool shown) IsShown(SerializedProperty property,
            ISaintsAttribute saintsAttribute, FieldInfo info,
            Type type, object target)
        {
            HideIfAttribute hideIfAttribute = (HideIfAttribute)saintsAttribute;

            List<bool> callbackTruly = new List<bool>();
            List<string> errors = new List<string>();

            foreach (string andCallback in hideIfAttribute.orCallbacks)
            {
                (string error, bool isTruly) = IsTruly(target, type, andCallback);
                if (error != "")
                {
                    errors.Add(error);
                }
                callbackTruly.Add(isTruly);
            }

            if (errors.Count > 0)
            {
                return (string.Join("\n\n", errors), true);
            }

            // empty means hide
            if (callbackTruly.Count == 0)
            {
                return ("", false);
            }

            // or, get hide
            bool truly = callbackTruly.Any(each => each);

            // reverse, get shown
            return ("", !truly);
        }
    }
}
