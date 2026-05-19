using Opsive.UltimateCharacterController.Game;
using UnityEngine;

/// <summary>
/// Damages characters that overlap the hand collider while a slam is active.
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class GiantHandSlamDamage : MonoBehaviour
{
    private static readonly Collider[] s_OverlapResults = new Collider[16];

    [SerializeField] private BoxCollider m_Collider;
    [SerializeField] private float m_HitDuration = 0.65f;

    private bool m_Active;
    private float m_Damage;
    private float m_ForceMagnitude;
    private GameObject m_Attacker;
    private LayerMask m_TargetLayers = 1 << LayerManager.Character;
    private readonly System.Collections.Generic.HashSet<Opsive.UltimateCharacterController.Traits.Health> m_HitThisSlam = new();

    private void Awake()
    {
        if (m_Collider == null) {
            m_Collider = GetComponent<BoxCollider>();
        }
        m_Collider.isTrigger = true;
        m_Collider.enabled = false;
    }

    public void BeginSlam(float damage, float forceMagnitude, GameObject attacker, LayerMask targetLayers)
    {
        m_Damage = damage;
        m_ForceMagnitude = forceMagnitude;
        m_Attacker = attacker;
        m_TargetLayers = targetLayers;
        m_HitThisSlam.Clear();
        m_Active = true;
        m_Collider.enabled = true;
        ApplyOverlapDamage();
        CancelInvoke(nameof(EndSlam));
        Invoke(nameof(EndSlam), m_HitDuration);
    }

    public void EndSlam()
    {
        m_Active = false;
        if (m_Collider != null) {
            m_Collider.enabled = false;
        }
    }

    private void FixedUpdate()
    {
        if (!m_Active) {
            return;
        }

        ApplyOverlapDamage();
    }

    private void ApplyOverlapDamage()
    {
        if (m_Collider == null || !m_Collider.enabled) {
            return;
        }

        var center = m_Collider.bounds.center;
        var halfExtents = m_Collider.bounds.extents;
        var count = Physics.OverlapBoxNonAlloc(center, halfExtents, s_OverlapResults, m_Collider.transform.rotation, m_TargetLayers, QueryTriggerInteraction.Collide);
        for (var i = 0; i < count; i++) {
            AreaDamageUtility.TryApplyDamage(s_OverlapResults[i], center, m_Damage, m_ForceMagnitude, m_Attacker, m_TargetLayers, m_HitThisSlam, requireGrounded: false);
        }
    }

    private void OnDisable()
    {
        EndSlam();
    }
}
