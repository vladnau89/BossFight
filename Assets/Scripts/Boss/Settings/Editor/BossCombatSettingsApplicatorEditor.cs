using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BossCombatSettingsApplicator))]
public sealed class BossCombatSettingsApplicatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        using (new EditorGUI.DisabledScope(serializedObject.targetObjects.Length == 0)) {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Apply", GUILayout.Height(24))) {
                foreach (var obj in serializedObject.targetObjects) {
                    if (obj is not BossCombatSettingsApplicator applicator) {
                        continue;
                    }

                    applicator.Apply();

                    if (!Application.isPlaying) {
                        MarkTargetsDirty(applicator);
                    }
                }
            }

            if (GUILayout.Button("Reset Defaults", GUILayout.Height(24))) {
                foreach (var obj in serializedObject.targetObjects) {
                    if (obj is not BossCombatSettingsApplicator applicator) {
                        continue;
                    }

                    var settings = GetAssignedSettings(applicator);
                    if (settings == null) {
                        continue;
                    }

                    Undo.RecordObject(settings, "Reset Boss Combat Settings");
                    settings.ResetToDefaults();
                    EditorUtility.SetDirty(settings);
                }

                serializedObject.Update();
            }

            EditorGUILayout.EndHorizontal();
        }

        if (Application.isPlaying) {
            EditorGUILayout.HelpBox(
                "Press Apply after changing the settings asset. You can also press Apply on the BossCombatSettings asset itself.",
                MessageType.Info);
        }
    }

    private static BossCombatSettings GetAssignedSettings(BossCombatSettingsApplicator applicator)
    {
        var serialized = new SerializedObject(applicator);
        var property = serialized.FindProperty("_settings");
        return property != null ? property.objectReferenceValue as BossCombatSettings : null;
    }

    internal static void MarkTargetsDirty(BossCombatSettingsApplicator applicator)
    {
        EditorUtility.SetDirty(applicator);

        var serialized = new SerializedObject(applicator);
        var iterator = serialized.GetIterator();
        var enterChildren = true;
        while (iterator.NextVisible(enterChildren)) {
            enterChildren = false;
            if (iterator.propertyType != SerializedPropertyType.ObjectReference) {
                continue;
            }

            if (iterator.objectReferenceValue is Component component) {
                EditorUtility.SetDirty(component);
            }
        }
    }
}
