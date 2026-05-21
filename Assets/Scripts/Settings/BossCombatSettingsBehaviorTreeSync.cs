using BehaviorDesigner.Runtime;
using UnityEngine;

/// <summary>
/// Pushes cooldown values from <see cref="BossCombatSettings"/> into the boss behavior tree.
/// Lives outside BossFight.Boss because that assembly cannot reference Behavior Designer.
/// </summary>
[DisallowMultipleComponent]
public sealed class BossCombatSettingsBehaviorTreeSync : MonoBehaviour, IBossCombatSettingsBehaviorTreeSync
{
    public const string HandSlamCooldownVariable = "Hand Slam Cooldown";
    public const string ChestPulseCooldownVariable = "ChestPulseCooldown";

    [SerializeField] private BehaviorTree _behaviorTree;

    public void ApplyBehaviorTreeSettings(float handSlamCooldown, float chestPulseCooldown, bool enableLog)
    {
        if (_behaviorTree == null) {
            return;
        }

        _behaviorTree.SetVariableValue(HandSlamCooldownVariable, handSlamCooldown);
        _behaviorTree.SetVariableValue(ChestPulseCooldownVariable, chestPulseCooldown);
        SetVariableIfExists(BossCombatLogUtility.EnableLogVariable, enableLog);
    }

    private void SetVariableIfExists(string variableName, bool value)
    {
        if (_behaviorTree.GetVariable(variableName) == null) {
            Debug.LogWarning(
                $"Boss combat: behavior tree '{_behaviorTree.name}' has no shared variable '{variableName}'. "
                + "Add a Shared Bool with that name (e.g. on BT_Boss / BT_Boss_Test).",
                _behaviorTree);
            return;
        }

        _behaviorTree.SetVariableValue(variableName, value);
    }
}
