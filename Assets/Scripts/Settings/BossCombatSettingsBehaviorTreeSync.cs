using BehaviorDesigner.Runtime;
using UnityEngine;

/// <summary>
/// Pushes cooldown values from <see cref="BossCombatSettings"/> into the boss behavior tree.
/// Lives outside BossFight.Boss because that assembly cannot reference Behavior Designer.
/// </summary>
[DisallowMultipleComponent]
public sealed class BossCombatSettingsBehaviorTreeSync : MonoBehaviour, IBossCombatSettingsBehaviorTreeSync
{
    public const string HandSlamCooldownVariable = "HandSlamCooldown";
    public const string ChestPulseCooldownVariable = "ChestPulseCooldown";
    public const string SearchDistanceVariable = "SearchDistance";
    public const string AttackDistanceVariable = "AttackDistance";

    [SerializeField] private BehaviorTree _behaviorTree;

    public void ApplyBehaviorTreeSettings(
        float handSlamCooldown,
        float chestPulseCooldown,
        bool enableLog,
        float searchDistance,
        float attackDistance)
    {
        BossCombatDebugLog.SetEnabled(enableLog);

        if (_behaviorTree == null) {
            return;
        }

        SetVariableIfExists(HandSlamCooldownVariable, handSlamCooldown);
        SetVariableIfExists(ChestPulseCooldownVariable, chestPulseCooldown);
        SetVariableIfExists(BossCombatLogUtility.EnableLogVariable, enableLog);
        SetVariableIfExists(SearchDistanceVariable, searchDistance);
        SetVariableIfExists(AttackDistanceVariable, attackDistance);
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

    private void SetVariableIfExists(string variableName, float value)
    {
        if (_behaviorTree.GetVariable(variableName) == null) {
            Debug.LogWarning(
                $"Boss combat: behavior tree '{_behaviorTree.name}' has no shared variable '{variableName}'. "
                + "Add a Shared Float with that name (e.g. on BT_Boss).",
                _behaviorTree);
            return;
        }

        _behaviorTree.SetVariableValue(variableName, value);
    }
}
