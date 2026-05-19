using System.Collections;
using UnityEngine;

/// <summary>
/// Weak spot on the giant hand. Absorbs damage until destroyed, then deals burst damage to the boss.
/// </summary>
public class WeakPointMarker : MonoBehaviour
{
    [SerializeField] private float m_MaxHealth = 30f;
    [SerializeField] private float m_BossDamageOnDestroy = 100f;
    [SerializeField] private SphereCollider m_Collider;

    private float m_CurrentHealth;
    private bool m_IsDestroyed;
    private BossCombat m_BossCombat;
    private WeakPointVisual m_Visual;
    private Coroutine m_HideCoroutine;

    public bool IsDestroyed => m_IsDestroyed;
    public Collider Collider => m_Collider != null ? m_Collider : m_Collider = GetComponent<SphereCollider>();

    private void Awake()
    {
        m_BossCombat = GetComponentInParent<BossCombat>();
        m_Visual = GetComponent<WeakPointVisual>();
        if (m_Visual == null) {
            m_Visual = gameObject.AddComponent<WeakPointVisual>();
        }
        ResetHealth();
    }

    public void ResetHealth()
    {
        if (m_HideCoroutine != null) {
            StopCoroutine(m_HideCoroutine);
            m_HideCoroutine = null;
        }
        m_IsDestroyed = false;
        m_CurrentHealth = m_MaxHealth;
        m_Visual?.ResetVisual();
    }

    /// <returns>True if this collider handled the hit.</returns>
    public bool TakeDamage(float amount, Vector3 position, Vector3 direction, float forceMagnitude, int frames, GameObject attacker, object attackerObject, Collider hitCollider)
    {
        if (m_IsDestroyed || !isActiveAndEnabled || !gameObject.activeInHierarchy) {
            return false;
        }

        m_CurrentHealth -= amount;
        m_Visual?.PlayHitFlash();
        if (m_CurrentHealth > 0f) {
            return true;
        }

        DestroyWeakPoint(position, direction, attacker, attackerObject);
        return true;
    }

    private void DestroyWeakPoint(Vector3 position, Vector3 direction, GameObject attacker, object attackerObject)
    {
        m_IsDestroyed = true;
        m_CurrentHealth = 0f;

        if (Collider != null) {
            Collider.enabled = false;
        }

        m_Visual?.PlayDestroyed();

        if (m_BossCombat != null && m_BossDamageOnDestroy > 0f) {
            m_BossCombat.ApplyWeakPointBurstDamage(m_BossDamageOnDestroy, position, direction, attacker, attackerObject);
        }

        m_BossCombat?.OnWeakPointDestroyed(this);

        if (m_HideCoroutine != null) {
            StopCoroutine(m_HideCoroutine);
        }
        m_HideCoroutine = StartCoroutine(HideAfterDestroyFlash());
    }

    private IEnumerator HideAfterDestroyFlash()
    {
        var duration = m_Visual != null ? m_Visual.DestroyFlashDuration : 0.25f;
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
        m_HideCoroutine = null;
    }
}
