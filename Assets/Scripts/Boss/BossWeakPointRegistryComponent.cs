using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Combat-wide weak point collider map and damage routing.
/// </summary>
[DisallowMultipleComponent]
public sealed class BossWeakPointRegistryComponent : MonoBehaviour
{
    [SerializeField] private BossPhaseWeakPointsComponent[] _phaseWeakPoints;
    [SerializeField] private BossCharacterHealth _health;

    private readonly Dictionary<Collider, WeakPointEntity> _weakPointByCollider = new();

    private void Awake() => BuildMap();

    private void BuildMap()
    {
        _weakPointByCollider.Clear();
        foreach (var phaseWeakPoints in _phaseWeakPoints) {
            phaseWeakPoints.RegisterWeakPoints(_weakPointByCollider);
        }
    }

    public bool IsWeakPointCollider(Collider hitCollider) => TryGetWeakPoint(hitCollider, out _);

    public bool TryDamageWeakPoint(Collider hitCollider, float amount, Vector3 position, Vector3 direction,
        float forceMagnitude, int frames, GameObject attacker, object attackerObject)
    {
        if (!TryGetWeakPoint(hitCollider, out var weakPoint)) {
            return false;
        }

        var wasDamaged = weakPoint.TakeDamage(amount);
        if (wasDamaged && weakPoint.IsDestroyed) {
            _health.ApplyWeakPointBurstDamage(weakPoint.BossDamageOnDestroy, position, direction, attacker,
                attackerObject);
        }

        return wasDamaged;
    }

    private bool TryGetWeakPoint(Collider hitCollider, out WeakPointEntity weakPoint) =>
        _weakPointByCollider.TryGetValue(hitCollider, out weakPoint);
}
