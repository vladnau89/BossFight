using Opsive.UltimateCharacterController.Traits;
using UnityEngine;

/// <summary>
/// Boss only takes damage from destroyed weak points. Body hits and AoE are ignored.
/// </summary>
public class BossCharacterHealth : CharacterHealth
{
    [SerializeField] private BossWeakPointRegistryComponent _weakPointRegistry;

    private bool _allowBossDamage;

    public void ApplyWeakPointBurstDamage(float amount, Vector3 position, Vector3 direction, GameObject attacker,
        object attackerObject)
    {
        _allowBossDamage = true;
        base.OnDamage(amount, position, direction, 0f, 0, 0f, attacker, attackerObject, null);
        _allowBossDamage = false;
    }

    public override void OnDamage(float amount, Vector3 position, Vector3 direction, float forceMagnitude, int frames,
        float radius, GameObject attacker, object attackerObject, Collider hitCollider)
    {
        if (hitCollider != null) {
            if (_weakPointRegistry.IsWeakPointCollider(hitCollider)) {
                _weakPointRegistry.TryDamageWeakPoint(hitCollider, amount, position, direction, forceMagnitude, frames,
                    attacker, attackerObject);
            }

            return;
        }

        if (radius > 0f || !_allowBossDamage) {
            return;
        }

        base.OnDamage(amount, position, direction, forceMagnitude, frames, radius, attacker, attackerObject, hitCollider);
    }
}
