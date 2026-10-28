#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NumberExposer))]
public class NumberExposerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw label, source, format, etc. — but NOT exposedProp (it's Hidden).
        DrawDefaultInspector();

        var so         = new SerializedObject(target);
        var sourceProp = so.FindProperty("source");
        var propProp   = so.FindProperty("exposedProp");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Exposed Member", EditorStyles.boldLabel);

        var source = sourceProp.objectReferenceValue as MonoBehaviour;

        if (source == null)
        {
            EditorGUILayout.HelpBox("Assign a Source component first.", MessageType.Info);
            return;
        }

        var names = GetNumericMemberNames(source.GetType());

        if (names.Count == 0)
        {
            EditorGUILayout.HelpBox(
                $"'{source.GetType().Name}' has no int/float fields or properties.",
                MessageType.Warning);
            return;
        }

        // Include a "(none)" entry so the user can clear it
        var options = new List<string> { "(none)" };
        options.AddRange(names);

        int current = Mathf.Max(0, options.IndexOf(propProp.stringValue));
        int picked  = EditorGUILayout.Popup("Member", current, options.ToArray());

        if (picked != current)
        {
            propProp.stringValue = picked == 0 ? "" : options[picked];
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }

        // Show current value for a quick sanity check
        if (!string.IsNullOrEmpty(propProp.stringValue))
        {
            object val = ReadValue(source, propProp.stringValue);
            EditorGUILayout.LabelField("Current Value", val?.ToString() ?? "(null)");
        }
    }

    private static List<string> GetNumericMemberNames(Type t)
    {
        const BindingFlags flags =
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

        var result = new List<string>();

        foreach (var f in t.GetFields(flags))
            if (IsNumeric(f.FieldType)) result.Add(f.Name);

        foreach (var p in t.GetProperties(flags))
            if (p.CanRead && IsNumeric(p.PropertyType)) result.Add(p.Name);

        return result.Distinct().OrderBy(n => n).ToList();
    }

    private static object ReadValue(MonoBehaviour src, string name)
    {
        var t = src.GetType();
        const BindingFlags flags =
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

        var f = t.GetField(name, flags);
        if (f != null) return f.GetValue(src);

        var p = t.GetProperty(name, flags);
        if (p != null && p.CanRead) return p.GetValue(src);

        return null;
    }

    private static bool IsNumeric(Type t) =>
        t == typeof(int)    || t == typeof(float)  || t == typeof(double) ||
        t == typeof(long)   || t == typeof(short)  || t == typeof(decimal) ||
        t == typeof(byte)   || t == typeof(sbyte)  || t == typeof(uint)   ||
        t == typeof(ulong)  || t == typeof(ushort);
}
#endif