using BehaviorDesigner.Runtime;
using UnityEngine;

/// <summary>
/// BT-aware helpers for <see cref="BehaviorDesigner.Runtime.Tasks.UltimateCharacterController.BossCombatLog"/>.
/// </summary>
public static class BossCombatLogUtility
{
    public const string EnableLogVariable = "EnableLog";

    public static void SetLogEnabled(bool enabled) => BossCombatDebugLog.SetEnabled(enabled);

    public static bool IsLogEnabled(Behavior behavior = null)
    {
        if (BossCombatDebugLog.IsEnabled) {
            return true;
        }

        return behavior != null && IsEnabled(behavior);
    }

    public static bool IsEnabled(Behavior behavior)
    {
        if (behavior == null) {
            return false;
        }

        var variable = behavior.GetVariable(EnableLogVariable);
        return variable is SharedBool sharedBool && sharedBool.Value;
    }

    public static void Log(string message, Object context = null, bool includeTime = true)
    {
        BossCombatDebugLog.Log(message, context, includeTime);
    }
}
