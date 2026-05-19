using Opsive.UltimateCharacterController.Traits;
using UnityEngine;

/// <summary>
/// Routes direct hits on weak point colliders to <see cref="WeakPointMarker"/> instead of boss health.
/// </summary>
public class BossCharacterHealth : CharacterHealth
{
    private BossCombat m_BossCombat;

    protected override void Awake()
    {
        base.Awake();
        m_BossCombat = GetComponent<BossCombat>();
    }

    public override void OnDamage(float amount, Vector3 position, Vector3 direction, float forceMagnitude, int frames, float radius, GameObject attacker, object attackerObject, Collider hitCollider)
    {
        if (m_BossCombat != null && hitCollider != null
            && m_BossCombat.TryDamageWeakPoint(hitCollider, amount, position, direction, forceMagnitude, frames, attacker, attackerObject)) {
            return;
        }

        base.OnDamage(amount, position, direction, forceMagnitude, frames, radius, attacker, attackerObject, hitCollider);
    }
}
