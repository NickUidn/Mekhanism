using System;
using System.Reflection;
using TMPro;
using UnityEngine;

public class NumberExposer : MonoBehaviour
{
    [SerializeField] private TMP_Text label;

    [Tooltip("Drag any MonoBehaviour here. Its numeric members appear in the dropdown below.")]
    [SerializeField] private MonoBehaviour source;

    // Filled by the custom editor's dropdown. Hidden in default inspector drawing.
    [SerializeField, HideInInspector] private string exposedProp = "";

    [SerializeField]
    [Tooltip(
    "How the value is formatted via string.\n" +
    "\n" +
    "float 87.5 examples:\n" +
    "  {0}          ->  87.5\n" +
    "  HP: {0}      ->  HP: 87.5\n" +
    "  {0:F0}       ->  88        (rounded)\n" +
    "  {0:F1}       ->  87.5\n" +
    "  {0:F2}       ->  87.50\n" +
    "  {0:0.00}     ->  87.50\n" +
    "  {0}%         ->  87.5%\n" +
    "  ${0:0.00}    ->  $87.50\n" +
    "  Ammo: {0}/30 ->  Ammo: 87.5/30\n" +
    "\n" +
    "int 42 examples:\n" +
    "  {0}          ->  42\n" +
    "  Score: {0}   ->  Score: 42\n" +
    "  {0:D3}       ->  042       (zero-padded)\n" +
    "  {0:N0}       ->  42\n" +
    "  Coins: {0:N0}->  Coins: 42\n" +
    "\n" +
    "Leave empty to show the raw number."
)]
    private string format = "{0}";

    private Func<object> getter;

    private void OnEnable() => CacheGetter();
    private void OnValidate() => CacheGetter();

    private void Update()
    {
        if (getter == null || label == null) return;
        label.text = string.Format(format, getter());
    }

    // Keep your original API — pass a raw string and show it
    public void Exposer(string text) => label.text = text;

    private void CacheGetter()
    {
        getter = null;
        if (source == null || string.IsNullOrEmpty(exposedProp)) return;

        Type t = source.GetType();
        const BindingFlags flags =
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

        FieldInfo f = t.GetField(exposedProp, flags);
        if (f != null && IsNumeric(f.FieldType)) { getter = () => f.GetValue(source); return; }

        PropertyInfo p = t.GetProperty(exposedProp, flags);
        if (p != null && p.CanRead && IsNumeric(p.PropertyType)) { getter = () => p.GetValue(source); return; }

        Debug.LogWarning($"[NumberExposer] '{exposedProp}' is not a numeric field/property on {t.Name}.", this);
    }

    private static bool IsNumeric(Type t) =>
        t == typeof(int) || t == typeof(float) || t == typeof(double) ||
        t == typeof(long) || t == typeof(short) || t == typeof(decimal) ||
        t == typeof(byte) || t == typeof(sbyte) || t == typeof(uint) ||
        t == typeof(ulong) || t == typeof(ushort);
}