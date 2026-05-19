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
    [SerializeField] private WeakPointVisual m_Visual;

    private float m_CurrentHealth;
    private Coroutine m_HideCoroutine;

    public Collider Collider => m_Collider;

    public float BossDamageOnDestroy => m_BossDamageOnDestroy;
    public bool IsDestroyed => m_CurrentHealth <= 0;

    private void Awake()
    {
        m_CurrentHealth = m_MaxHealth;
    }
    
    public void ResetHealth()
    {
        if (m_HideCoroutine != null) {
            StopCoroutine(m_HideCoroutine);
            m_HideCoroutine = null;
        }
        m_CurrentHealth = m_MaxHealth;
        EnableCollider(true);
        m_Visual?.ResetVisual();
    }

    /// <returns>True if this collider handled the hit.</returns>
    public bool TakeDamage(float amount)
    {
        if (m_CurrentHealth <= 0 || !isActiveAndEnabled || !gameObject.activeInHierarchy) {
            return false;
        }

        m_CurrentHealth -= amount;
        m_Visual?.PlayHitFlash();
        if (m_CurrentHealth > 0f) {
            return true;
        }

        DestroyWeakPoint();
        return true;
    }

    public void SetActive(bool active) => gameObject.SetActive(active);

    private void DestroyWeakPoint()
    {
        m_CurrentHealth = 0f;

        EnableCollider(false);

        m_Visual?.PlayDestroyed();
        
        if (m_HideCoroutine != null) {
            StopCoroutine(m_HideCoroutine);
        }
        m_HideCoroutine = StartCoroutine(HideAfterDestroyFlash());
    }

    private void EnableCollider(bool enable) => m_Collider.enabled = enable;

    private IEnumerator HideAfterDestroyFlash()
    {
        var duration = m_Visual != null ? m_Visual.DestroyFlashDuration : 0.25f;
        yield return new WaitForSeconds(duration);
        SetActive(false);
        m_HideCoroutine = null;
    }
}
