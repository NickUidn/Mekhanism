#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class ResourceDebugWindow : EditorWindow
{
    private int scrapInput;
    [MenuItem("Tools/Resource Debug")]
    static void Open() => GetWindow<ResourceDebugWindow>("Resource Debug");

    private void OnEnable() => SyncFromManager();
    private void OnFocus() => SyncFromManager();
    private void SyncFromManager()
    {
        scrapInput = ResourceManager.scrap;
    }


    void OnGUI()
    {
        
        EditorGUILayout.LabelField("Overrides", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        ResourceManager.TESTING_FUNCTIONALITY = EditorGUILayout.Toggle("TESTING", ResourceManager.TESTING_FUNCTIONALITY);
        ResourceManager.OVERRIDE_COOLING = EditorGUILayout.Toggle("Override Cooling", ResourceManager.OVERRIDE_COOLING);
        ResourceManager.OVERRIDE_FUEL = EditorGUILayout.Toggle("Override Fuel", ResourceManager.OVERRIDE_FUEL);
        ResourceManager.OVERRIDE_WEAR_AND_TEAR = EditorGUILayout.Toggle("Override Wear And Tear", ResourceManager.OVERRIDE_WEAR_AND_TEAR);
        EditorGUILayout.Space(8);
                     
        // Editable field with instant apply
        EditorGUI.BeginChangeCheck();
        scrapInput = EditorGUILayout.IntField("Scrap", scrapInput);
        if (EditorGUI.EndChangeCheck())
        {
            ResourceManager.scrap = scrapInput;   // setter clamps 0..ScrapMax
        }
    }
}
#endif