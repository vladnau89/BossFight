using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Character.Abilities;
using Opsive.UltimateCharacterController.Traits;
using UnityEngine;

/// <summary>
/// Ground checks for ground-only attacks (shockwaves).
/// </summary>
public static class CharacterGroundedUtility
{
    /// <summary>
    /// True when the character is on the ground and not in a jump (can be avoided by jumping).
    /// </summary>
    public static bool CanTakeGroundWaveDamage(Health health, float maxHeightAboveGround = 0.45f)
    {
        if (health == null) {
            return false;
        }

        var locomotion = health.GetComponentInParent<CharacterLocomotion>();
        if (locomotion != null && !locomotion.Grounded) {
            return false;
        }

        var ultimateLocomotion = health.GetComponentInParent<UltimateCharacterLocomotion>();
        if (ultimateLocomotion != null && ultimateLocomotion.IsAbilityTypeActive<Jump>()) {
            return false;
        }

        var feet = health.transform.position;
        var collider = health.GetComponent<Collider>();
        if (collider != null) {
            feet = collider.bounds.min;
        }

        if (Physics.Raycast(feet + Vector3.up * 0.05f, Vector3.down, out var hit, maxHeightAboveGround + 1f, ~0, QueryTriggerInteraction.Ignore)) {
            return (feet.y - hit.point.y) <= maxHeightAboveGround;
        }

        return locomotion != null && locomotion.Grounded;
    }
}
