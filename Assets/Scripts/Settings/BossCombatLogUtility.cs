using BehaviorDesigner.Runtime;

/// <summary>
/// Shared helpers for <see cref="BehaviorDesigner.Runtime.Tasks.UltimateCharacterController.BossCombatLog"/>.
/// </summary>
public static class BossCombatLogUtility
{
    public const string EnableLogVariable = "EnableLog";

    public static bool IsEnabled(Behavior behavior)
    {
        if (behavior == null) {
            return false;
        }

        var variable = behavior.GetVariable(EnableLogVariable);
        return variable is SharedBool sharedBool && sharedBool.Value;
    }
}
