using UnityEngine;

/// <summary>
/// Combat debug log gate and writers. Lives in BossFight.Boss so gameplay code can log without Behavior Designer.
/// </summary>
public static class BossCombatDebugLog
{
    private static bool s_enabled;

    public static void SetEnabled(bool enabled) => s_enabled = enabled;

    public static bool IsEnabled => s_enabled;

    public static void Log(string message, Object context = null, bool includeTime = true)
    {
        if (!s_enabled) {
            return;
        }

        if (includeTime) {
            message = $"{Time.time:F2}: {message}";
        }

        Debug.Log(message, context);
    }
}
