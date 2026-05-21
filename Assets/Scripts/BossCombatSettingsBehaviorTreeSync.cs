using BehaviorDesigner.Runtime;
using UnityEngine;

/// <summary>
/// Pushes cooldown values from <see cref="BossCombatSettings"/> into the boss behavior tree.
/// </summary>
[DisallowMultipleComponent]
public sealed class BossCombatSettingsBehaviorTreeSync : MonoBehaviour, IBossCombatSettingsBehaviorTreeSync
{
    public const string HandSlamCooldownVariable = "Hand Slam Cooldown";
    public const string ChestPulseCooldownVariable = "ChestPulseCooldown";

    [SerializeField] private BehaviorTree _behaviorTree;

    public void ApplyBehaviorTreeSettings(float handSlamCooldown, float chestPulseCooldown)
    {
        _behaviorTree.SetVariableValue(HandSlamCooldownVariable, handSlamCooldown);
        _behaviorTree.SetVariableValue(ChestPulseCooldownVariable, chestPulseCooldown);
    }
}
