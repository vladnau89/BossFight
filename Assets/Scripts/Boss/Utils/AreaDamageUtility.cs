using System.Collections.Generic;
using Opsive.UltimateCharacterController.Game;
using Opsive.UltimateCharacterController.Traits;
using Opsive.UltimateCharacterController.Utility;
using UnityEngine;

/// <summary>
/// Applies UCC health damage to characters in an overlap shape.
/// </summary>
public static class AreaDamageUtility
{
    private static readonly Collider[] s_Colliders = new Collider[32];

    /// <param name="damagedTargets">Targets already hit by this wave; persists for the wave lifetime so each character is damaged at most once.</param>
    public static void DamageRing(Vector3 center, float innerRadius, float outerRadius, float damage, float forceMagnitude, GameObject attacker, LayerMask layers, HashSet<Health> damagedTargets, bool requireGrounded = false)
    {
        var count = Physics.OverlapSphereNonAlloc(center, outerRadius, s_Colliders, layers, QueryTriggerInteraction.Collide);
        var innerSqr = innerRadius * innerRadius;
        var outerSqr = outerRadius * outerRadius;
        for (var i = 0; i < count; i++) {
            var collider = s_Colliders[i];
            var closest = collider.ClosestPoint(center);
            var sqrDistance = (closest - center).sqrMagnitude;
            if (sqrDistance < innerSqr || sqrDistance > outerSqr) {
                continue;
            }
            TryApplyDamage(collider, closest, damage, forceMagnitude, attacker, layers, damagedTargets, requireGrounded);
        }
    }

    public static bool TryApplyDamage(Collider collider, Vector3 position, float damage, float forceMagnitude, GameObject attacker, LayerMask layers, HashSet<Health> damagedSet, bool requireGrounded)
    {
        if (!MathUtility.InLayerMask(collider.gameObject.layer, layers)) {
            return false;
        }

        var health = collider.GetComponentInParent<Health>();
        if (health == null || !damagedSet.Add(health)) {
            return false;
        }

        if (requireGrounded && !CharacterGroundedUtility.CanTakeGroundWaveDamage(health)) {
            damagedSet.Remove(health);
            return false;
        }

        var direction = health.transform.position - position;
        if (direction.sqrMagnitude < 0.001f) {
            direction = Vector3.up;
        } else {
            direction.Normalize();
        }

        health.Damage(damage, position, direction, forceMagnitude, 1, 0f, attacker, collider);
        return true;
    }
}
