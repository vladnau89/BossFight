using Opsive.UltimateCharacterController.Traits;
using UnityEngine;

/// <summary>
/// Boss only takes damage from destroyed weak points. Body hits and AoE are ignored.
/// </summary>
public class BossCharacterHealth : CharacterHealth
{
    private BossCombat m_BossCombat;
    private bool m_AllowBossDamage;

    protected override void Awake()
    {
        base.Awake();
        m_BossCombat = GetComponentInChildren<BossCombat>();
    }

    public void ApplyWeakPointBurstDamage(float amount, Vector3 position, Vector3 direction, GameObject attacker, object attackerObject)
    {
        m_AllowBossDamage = true;
        base.OnDamage(amount, position, direction, 0f, 0, 0f, attacker, attackerObject, null);
        m_AllowBossDamage = false;
    }

    public override void OnDamage(float amount, Vector3 position, Vector3 direction, float forceMagnitude, int frames, float radius, GameObject attacker, object attackerObject, Collider hitCollider)
    {
        if (m_BossCombat != null && hitCollider != null) {
            if (m_BossCombat.IsWeakPointCollider(hitCollider)) {
                m_BossCombat.TryDamageWeakPoint(hitCollider, amount, position, direction, forceMagnitude, frames, attacker, attackerObject);
            }
            return;
        }

        if (radius > 0f || !m_AllowBossDamage) {
            return;
        }

        base.OnDamage(amount, position, direction, forceMagnitude, frames, radius, attacker, attackerObject, hitCollider);
    }
}
