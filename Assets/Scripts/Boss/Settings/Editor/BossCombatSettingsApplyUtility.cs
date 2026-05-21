using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

internal static class BossCombatSettingsApplyUtility
{
    public static int Apply(BossCombatSettings settings)
    {
        if (settings == null) {
            return 0;
        }

        var count = 0;

        foreach (var applicator in FindSceneApplicators(settings)) {
            ApplyApplicator(applicator);
            count++;
        }

        count += ApplyInOpenPrefabStages(settings);

        if (count == 0 && !Application.isPlaying) {
            count += ApplyInPrefabs(settings);
        }

        return count;
    }

    private static void ApplyApplicator(BossCombatSettingsApplicator applicator)
    {
        applicator.Apply();

        if (!Application.isPlaying) {
            BossCombatSettingsApplicatorEditor.MarkTargetsDirty(applicator);
        }
    }

    private static IEnumerable<BossCombatSettingsApplicator> FindSceneApplicators(BossCombatSettings settings)
    {
        for (var i = 0; i < SceneManager.sceneCount; i++) {
            var scene = SceneManager.GetSceneAt(i);
            if (!scene.isLoaded) {
                continue;
            }

            foreach (var root in scene.GetRootGameObjects()) {
                foreach (var applicator in root.GetComponentsInChildren<BossCombatSettingsApplicator>(true)) {
                    if (GetAssignedSettings(applicator) == settings) {
                        yield return applicator;
                    }
                }
            }
        }
    }

    private static int ApplyInOpenPrefabStages(BossCombatSettings settings)
    {
        var count = 0;
        var stage = PrefabStageUtility.GetCurrentPrefabStage();
        if (stage == null) {
            return 0;
        }

        foreach (var root in stage.scene.GetRootGameObjects()) {
            foreach (var applicator in root.GetComponentsInChildren<BossCombatSettingsApplicator>(true)) {
                if (GetAssignedSettings(applicator) != settings) {
                    continue;
                }

                ApplyApplicator(applicator);
                count++;
            }
        }

        return count;
    }

    private static int ApplyInPrefabs(BossCombatSettings settings)
    {
        var count = 0;

        foreach (var guid in AssetDatabase.FindAssets("t:Prefab")) {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var root = PrefabUtility.LoadPrefabContents(path);

            try {
                var changed = false;

                foreach (var applicator in root.GetComponentsInChildren<BossCombatSettingsApplicator>(true)) {
                    if (GetAssignedSettings(applicator) != settings) {
                        continue;
                    }

                    applicator.Apply();
                    changed = true;
                    count++;
                }

                if (changed) {
                    PrefabUtility.SaveAsPrefabAsset(root, path);
                }
            } finally {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        return count;
    }

    private static BossCombatSettings GetAssignedSettings(BossCombatSettingsApplicator applicator)
    {
        var serialized = new SerializedObject(applicator);
        var property = serialized.FindProperty("_settings");
        return property != null ? property.objectReferenceValue as BossCombatSettings : null;
    }
}
