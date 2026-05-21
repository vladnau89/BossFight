using System;
using UnityEngine;

[Serializable]
public struct BossCombatPhase2Settings
{
    [Header("Behavior tree (seconds)")]
    [SerializeField] private float _chestPulseCooldown;

    [Header("Chest pulse — ground shockwave")]
    [SerializeField] private BossCombatShockwaveSettings _chestPulseShockwave;

    [Header("Weak points")]
    [SerializeField] private BossCombatWeakPointPhaseSettings _weakPoints;

    public float ChestPulseCooldown => _chestPulseCooldown;
    public BossCombatShockwaveSettings ChestPulseShockwave => _chestPulseShockwave;
    public BossCombatWeakPointPhaseSettings WeakPoints => _weakPoints;

    public void ApplyCombat(
        GroundShockwaveSpawner chestPulseShockwave,
        BossPhaseWeakPointsComponent weakPoints)
    {
        if (chestPulseShockwave != null) {
            chestPulseShockwave.ApplySettings(_chestPulseShockwave);
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

    public static BossCombatPhase2Settings Default => new BossCombatPhase2Settings
    {
        _chestPulseCooldown = 20f,
        _chestPulseShockwave = BossCombatShockwaveSettings.ChestPulseDefault,
        _weakPoints = BossCombatWeakPointPhaseSettings.Default,
    };
}
