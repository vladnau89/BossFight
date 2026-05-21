using Opsive.UltimateCharacterController.Items.Actions;
using Opsive.UltimateCharacterController.Traits;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Copies values from <see cref="BossCombatSettings"/> into wired combat components and behavior-tree variables.
/// Call <see cref="Apply"/> manually (inspector Apply button) or automatically on play via <see cref="_applyOnPlay"/>.
/// </summary>
[DisallowMultipleComponent]
public sealed class BossCombatSettingsApplicator : MonoBehaviour
{
    private const string DefaultHealthAttributeName = "Health";

    [FormerlySerializedAs("_tuning")]
    [SerializeField] private BossCombatSettings _settings;
    [SerializeField] private bool _applyOnPlay = true;

    [Header("Global")]
    [SerializeField] private BossPhaseHealthEnterCondition _phase2EnterCondition;
    [SerializeField] private AttributeManager _bossAttributeManager;
    [SerializeField] private string _bossHealthAttributeName = DefaultHealthAttributeName;
    [SerializeField] private ShootableWeapon _bossRocketWeapon;

    [FormerlySerializedAs("_behaviorTreeTuningSync")]
    [SerializeField] private MonoBehaviour _behaviorTreeSettingsSync;

    [Header("Phase 1")]
    [FormerlySerializedAs("_handSlamShockwave")]
    [SerializeField] private GroundShockwaveSpawner _phase1HandSlamShockwave;
    [FormerlySerializedAs("_handSlamDamageApplier")]
    [SerializeField] private GiantHandSlamDamageApplier _phase1HandSlamDamage;
    [SerializeField] private BossPhaseWeakPointsComponent _phase1WeakPoints;

    [Header("Phase 2")]
    [FormerlySerializedAs("_chestPulseShockwave")]
    [SerializeField] private GroundShockwaveSpawner _phase2ChestPulseShockwave;
    [SerializeField] private BossPhaseWeakPointsComponent _phase2WeakPoints;

    private void Start()
    {
        if (_applyOnPlay && Application.isPlaying) {
            Apply();
        }
    }

    [ContextMenu("Apply Settings")]
    public void Apply()
    {
        if (_settings == null) {
            return;
        }

        _phase2EnterCondition.ApplySettings(_settings.Phase2EnterHealthFraction);
        ApplyBossHealth(_settings.BossMaxHealth);

        _settings.Phase1.ApplyCombat(_phase1HandSlamShockwave, _phase1HandSlamDamage, _phase1WeakPoints);
        _settings.Phase2.ApplyCombat(_phase2ChestPulseShockwave, _phase2WeakPoints);

        BossCombatDebugLog.SetEnabled(_settings.EnableBehaviorTreeLog);

        if (_behaviorTreeSettingsSync is IBossCombatSettingsBehaviorTreeSync behaviorTreeSync) {
            behaviorTreeSync.ApplyBehaviorTreeSettings(
                _settings.Phase1.HandSlamCooldown,
                _settings.Phase2.ChestPulseCooldown,
                _settings.EnableBehaviorTreeLog,
                _settings.SearchDistance,
                _settings.AttackDistance);
        }

        if (_bossRocketWeapon != null) {
            _bossRocketWeapon.DamageAmount = _settings.BossRocketDamage;
        }
    }

    private void ApplyBossHealth(float maxHealth)
    {
        if (_bossAttributeManager == null) {
            return;
        }

        var attributeName = string.IsNullOrEmpty(_bossHealthAttributeName)
            ? DefaultHealthAttributeName
            : _bossHealthAttributeName;
        var health = _bossAttributeManager.GetAttribute(attributeName);
        if (health == null) {
            return;
        }

        health.MaxValue = maxHealth;
        health.Value = maxHealth;
    }
}
