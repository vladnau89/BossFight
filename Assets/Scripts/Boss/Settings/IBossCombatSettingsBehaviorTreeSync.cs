/// <summary>
/// Applies combat settings values to Behavior Designer shared variables (implemented outside BossFight.Boss assembly).
/// </summary>
public interface IBossCombatSettingsBehaviorTreeSync
{
    void ApplyBehaviorTreeSettings(float handSlamCooldown, float chestPulseCooldown);
}
