using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BossCombatSettings))]
public sealed class BossCombatSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space();

        var settings = (BossCombatSettings)target;

        using (new EditorGUI.DisabledScope(settings == null)) {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Apply", GUILayout.Height(24))) {
                var count = BossCombatSettingsApplyUtility.Apply(settings);

                if (count == 0) {
                    Debug.LogWarning(
                        $"No BossCombatSettingsApplicator uses '{settings.name}'. Assign it on the boss (Combat → Settings Applicator).",
                        settings);
                } else {
                    Debug.Log($"Applied '{settings.name}' to {count} applicator(s).", settings);
                }
            }

            if (GUILayout.Button("Reset Defaults", GUILayout.Height(24))) {
                Undo.RecordObject(settings, "Reset Boss Combat Settings");
                settings.ResetToDefaults();
                EditorUtility.SetDirty(settings);
                serializedObject.Update();
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.HelpBox(
            "Apply — push asset values into the boss. Reset Defaults — restore factory values on this asset (does not apply until you press Apply).",
            MessageType.Info);
    }
}
