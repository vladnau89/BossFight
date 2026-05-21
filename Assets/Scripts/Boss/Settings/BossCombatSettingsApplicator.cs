using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Copies values from <see cref="BossCombatSettings"/> into wired combat components and behavior-tree variables.
/// Call <see cref="Apply"/> manually (inspector Apply button).
/// </summary>
[DisallowMultipleComponent]
public sealed class BossCombatSettingsApplicator : MonoBehaviour
{
    [FormerlySerializedAs("_tuning")]
    [SerializeField] private BossCombatSettings _settings;

    [SerializeField] private BossPhaseHealthEnterCondition _phase2EnterCondition;
    [SerializeField] private GroundShockwaveSpawner _handSlamShockwave;
    [SerializeField] private GroundShockwaveSpawner _chestPulseShockwave;
    [SerializeField] private GiantHandSlamDamageApplier _handSlamDamageApplier;

    [FormerlySerializedAs("_behaviorTreeTuningSync")]
    [SerializeField] private MonoBehaviour _behaviorTreeSettingsSync;

    [ContextMenu("Apply Settings")]
    public void Apply()
    {
        if (_settings == null) {
            return;
        }

        _phase2EnterCondition.ApplySettings(_settings.Phase2EnterHealthFraction);
        _handSlamShockwave.ApplySettings(_settings.HandSlamShockwave);
        _chestPulseShockwave.ApplySettings(_settings.ChestPulseShockwave);
        _handSlamDamageApplier.ApplySettings(_settings.HandSlamDamage, _settings.HandSlamForce);

        if (_behaviorTreeSettingsSync is IBossCombatSettingsBehaviorTreeSync behaviorTreeSync) {
            behaviorTreeSync.ApplyBehaviorTreeSettings(_settings.HandSlamCooldown, _settings.ChestPulseCooldown);
        }
    }
}
