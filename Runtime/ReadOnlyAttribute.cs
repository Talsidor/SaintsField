﻿using System;
using System.Diagnostics;
using UnityEngine;

namespace SaintsField
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ReadOnlyAttribute: PropertyAttribute, ISaintsAttribute
    {
        public SaintsAttributeType AttributeType => SaintsAttributeType.Other;
        public string GroupBy => "";

        // ReSharper disable InconsistentNaming
        public readonly string[] Callbacks;
        public readonly EMode EditorMode;
        public readonly (string callback, Enum enumTarget)[] EnumTargets;
        // ReSharper enable InconsistentNaming

        public ReadOnlyAttribute(params string[] by): this(EMode.Edit | EMode.Play, by)
        {
        }

        public ReadOnlyAttribute(EMode editorMode, params string[] by)
        {
            EditorMode = editorMode;

            Callbacks = by;

            EnumTargets = Array.Empty<(string, Enum)>();
        }

        #region Enum 1-4

        public ReadOnlyAttribute(EMode editorMode, string callback1, object enumTarget1)
        {
            EditorMode = editorMode;
            Callbacks = Array.Empty<string>();
            EnumTargets = new[] {(callback1, (Enum)enumTarget1)};
        }

        public ReadOnlyAttribute(string callback1, object enumTarget1): this(EMode.Edit | EMode.Play, callback1, enumTarget1)
        {
        }

        public ReadOnlyAttribute(EMode editorMode, string callback1, object enumTarget1, string callback2, object enumTarget2)
        {
            EditorMode = editorMode;
            Callbacks = Array.Empty<string>();
            EnumTargets = new[] {(callback1, (Enum)enumTarget1), (callback2, (Enum)enumTarget2)};
        }

        public ReadOnlyAttribute(string callback1, object enumTarget1, string callback2, object enumTarget2): this(EMode.Edit | EMode.Play, callback1, enumTarget1, callback2, enumTarget2)
        {
        }

        public ReadOnlyAttribute(EMode editorMode, string callback1, object enumTarget1, string callback2, object enumTarget2, string callback3, object enumTarget3)
        {
            EditorMode = editorMode;
            Callbacks = Array.Empty<string>();
            EnumTargets = new[] {(callback1, (Enum)enumTarget1), (callback2, (Enum)enumTarget2), (callback3, (Enum)enumTarget3)};
        }

        public ReadOnlyAttribute(string callback1, object enumTarget1, string callback2, object enumTarget2, string callback3, object enumTarget3): this(EMode.Edit | EMode.Play, callback1, enumTarget1, callback2, enumTarget2, callback3, enumTarget3)
        {
        }

        public ReadOnlyAttribute(EMode editorMode, string callback1, object enumTarget1, string callback2, object enumTarget2, string callback3, object enumTarget3, string callback4, object enumTarget4)
        {
            EditorMode = editorMode;
            Callbacks = Array.Empty<string>();
            EnumTargets = new[] {(callback1, (Enum)enumTarget1), (callback2, (Enum)enumTarget2), (callback3, (Enum)enumTarget3), (callback4, (Enum)enumTarget4)};
        }

        public ReadOnlyAttribute(string callback1, object enumTarget1, string callback2, object enumTarget2, string callback3, object enumTarget3, string callback4, object enumTarget4): this(EMode.Edit | EMode.Play, callback1, enumTarget1, callback2, enumTarget2, callback3, enumTarget3, callback4, enumTarget4)
        {
        }
        #endregion

        #region string+enum 2-4

        // 1+1
        public ReadOnlyAttribute(EMode editorMode, string normalCallback, string enumCallback1, object enumTarget1)
        {
            EditorMode = editorMode;
            Callbacks = new[] {normalCallback};
            EnumTargets = new[] {(enumCallback1, (Enum)enumTarget1)};
        }
        public ReadOnlyAttribute(string normalCallback, string enumCallback1, object enumTarget1): this(EMode.Edit | EMode.Play, normalCallback, enumCallback1, enumTarget1)
        {
        }

        // 1+2
        public ReadOnlyAttribute(EMode editorMode, string normalCallback, string enumCallback1, object enumTarget1, string enumCallback2, object enumTarget2)
        {
            EditorMode = editorMode;
            Callbacks = new[] {normalCallback};
            EnumTargets = new[] {(enumCallback1, (Enum)enumTarget1), (enumCallback2, (Enum)enumTarget2)};
        }
        public ReadOnlyAttribute(string normalCallback, string enumCallback1, object enumTarget1, string enumCallback2, object enumTarget2): this(EMode.Edit | EMode.Play, normalCallback, enumCallback1, enumTarget1, enumCallback2, enumTarget2)
        {
        }

        // 1+3
        public ReadOnlyAttribute(EMode editorMode, string normalCallback, string enumCallback1, object enumTarget1, string enumCallback2, object enumTarget2, string enumCallback3, object enumTarget3)
        {
            EditorMode = editorMode;
            Callbacks = new[] {normalCallback};
            EnumTargets = new[] {(enumCallback1, (Enum)enumTarget1), (enumCallback2, (Enum)enumTarget2), (enumCallback3, (Enum)enumTarget3)};
        }
        public ReadOnlyAttribute(string normalCallback, string enumCallback1, object enumTarget1, string enumCallback2, object enumTarget2, string enumCallback3, object enumTarget3): this(EMode.Edit | EMode.Play, normalCallback, enumCallback1, enumTarget1, enumCallback2, enumTarget2, enumCallback3, enumTarget3)
        {
        }

        // 2+1
        public ReadOnlyAttribute(EMode editorMode, string normalCallback1, string normalCallback2, string enumCallback1, object enumTarget1)
        {
            EditorMode = editorMode;
            Callbacks = new[] {normalCallback1, normalCallback2};
            EnumTargets = new[] {(enumCallback1, (Enum)enumTarget1)};
        }
        public ReadOnlyAttribute(string normalCallback1, string normalCallback2, string enumCallback1, object enumTarget1): this(EMode.Edit | EMode.Play, normalCallback1, normalCallback2, enumCallback1, enumTarget1)
        {
        }


        // 2+2
        public ReadOnlyAttribute(EMode editorMode, string normalCallback1, string normalCallback2, string enumCallback1, object enumTarget1, string enumCallback2, object enumTarget2)
        {
            EditorMode = editorMode;
            Callbacks = new[] {normalCallback1, normalCallback2};
            EnumTargets = new[] {(enumCallback1, (Enum)enumTarget1), (enumCallback2, (Enum)enumTarget2)};
        }
        public ReadOnlyAttribute(string normalCallback1, string normalCallback2, string enumCallback1, object enumTarget1, string enumCallback2, object enumTarget2): this(EMode.Edit | EMode.Play, normalCallback1, normalCallback2, enumCallback1, enumTarget1, enumCallback2, enumTarget2)
        {
        }

        // 3+1
        public ReadOnlyAttribute(EMode editorMode, string normalCallback1, string normalCallback2, string normalCallback3, string enumCallback1, object enumTarget1)
        {
            EditorMode = editorMode;
            Callbacks = new[] {normalCallback1, normalCallback2, normalCallback3};
            EnumTargets = new[] {(enumCallback1, (Enum)enumTarget1)};
        }
        public ReadOnlyAttribute(string normalCallback1, string normalCallback2, string normalCallback3, string enumCallback1, object enumTarget1): this(EMode.Edit | EMode.Play, normalCallback1, normalCallback2, normalCallback3, enumCallback1, enumTarget1)
        {
        }

        #endregion
    }
}
