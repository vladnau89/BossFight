using Opsive.UltimateCharacterController.Traits;
using UnityEngine;

/// <summary>
/// Enter phase when boss health fraction is at or below the configured threshold.
/// </summary>
[DisallowMultipleComponent]
public sealed class BossPhaseHealthEnterCondition : BossPhaseEnterCondition
{
    [SerializeField] private Health _health;
    [SerializeField] [Range(0f, 1f)] private float _enterAtHealthFraction = 0.5f;

    public override bool ShouldEnter()
    {
        if (!_health.IsAlive()) {
            return false;
        }

        var healthFraction = _health.HealthValue / _health.HealthMaxValue;
        return healthFraction <= _enterAtHealthFraction;
    }
}
