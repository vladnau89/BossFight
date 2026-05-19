using System.Reflection;
using Opsive.UltimateCharacterController.Traits;
using UnityEngine;

/// <summary>
/// Weak spot collider on the giant hand. Used by <see cref="BossCombat"/> to register hitboxes.
/// </summary>
public class WeakPointMarker : MonoBehaviour
{
    [SerializeField] private float m_DamageMultiplier = 3f;
    [SerializeField] private SphereCollider m_Collider;

    public float DamageMultiplier => m_DamageMultiplier;
    public Collider Collider => m_Collider != null ? m_Collider : m_Collider = GetComponent<SphereCollider>();

    public Hitbox CreateHitbox() => new(Collider, m_DamageMultiplier);
}
