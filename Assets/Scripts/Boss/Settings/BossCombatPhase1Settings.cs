using System;
using UnityEngine;

[Serializable]
public struct BossCombatPhase1Settings
{
    [Header("Behavior tree (seconds)")]
    [SerializeField] private float _handSlamCooldown;

    [Header("Hand slam — ground shockwave")]
    [SerializeField] private BossCombatShockwaveSettings _handSlamShockwave;

    [Header("Hand slam — direct hit")]
    [SerializeField] private float _handSlamDamage;
    [SerializeField] private float _handSlamForce;

    [Header("Weak points")]
    [SerializeField] private BossCombatWeakPointPhaseSettings _weakPoints;

    public float HandSlamCooldown => _handSlamCooldown;
    public BossCombatShockwaveSettings HandSlamShockwave => _handSlamShockwave;
    public float HandSlamDamage => _handSlamDamage;
    public float HandSlamForce => _handSlamForce;
    public BossCombatWeakPointPhaseSettings WeakPoints => _weakPoints;

    public void ApplyCombat(
        GroundShockwaveSpawner handSlamShockwave,
        GiantHandSlamDamageApplier handSlamDamage,
        BossPhaseWeakPointsComponent weakPoints)
    {
        if (handSlamShockwave != null) {
            handSlamShockwave.ApplySettings(_handSlamShockwave);
        }

        if (handSlamDamage != null) {
            handSlamDamage.ApplySettings(_handSlamDamage, _handSlamForce);
        }

        ApplyWeakPoints(weakPoints);
    }

    public void ApplyWeakPoints(BossPhaseWeakPointsComponent weakPoints)
    {
        if (weakPoints == null) {
            return;
        }

        weakPoints.ApplyCombatSettings(_weakPoints.MaxHealth, _weakPoints.BossBurstDamage);
    }

    public static BossCombatPhase1Settings Default => new BossCombatPhase1Settings
    {
        _handSlamCooldown = 15f,
        _handSlamShockwave = BossCombatShockwaveSettings.HandSlamDefault,
        _handSlamDamage = 35f,
        _handSlamForce = 4f,
        _weakPoints = BossCombatWeakPointPhaseSettings.Default,
    };
}
