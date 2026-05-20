using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

internal static class WeakPointPlayModeTestAssets
{
    internal const string WeakPointPrefabPath = "Assets/Prefabs/Boss/WeakPoint.prefab";

    internal static GameObject LoadWeakPointPrefab()
    {
#if UNITY_EDITOR
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(WeakPointPrefabPath);
        if (prefab == null) {
            throw new InvalidOperationException($"Prefab not found at {WeakPointPrefabPath}");
        }

        return prefab;
#else
        throw new NotSupportedException("WeakPoint play mode tests run in the Unity Editor.");
#endif
    }
}
