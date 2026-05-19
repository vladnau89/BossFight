using System.Collections;
using Opsive.UltimateCharacterController.Traits;
using UnityEngine;

/// <summary>
/// Phase 2: chest cores and shock pulse attack.
/// </summary>
[DisallowMultipleComponent]
public class BossCombatPhase2 : BossCombatPhase
{
    [Header("Phase 2 pulse")]
    [SerializeField] private Transform m_PulseOrigin;
    [SerializeField] private GroundShockwave m_ShockwavePrefab;
    [SerializeField] private float m_Windup = 0.6f;
    [SerializeField] private float m_Damage = 18f;
    [SerializeField] private float m_MaxRadius = 14f;
    [SerializeField] private float m_Speed = 12f;
    [SerializeField] private float m_VulnerabilityDuration = 8f;

    [SerializeField] private BossCombat m_BossCombat;
    [SerializeField] private Health m_Health;
    private Coroutine m_PulseCoroutine;

    public Transform PulseOrigin => m_PulseOrigin;
    public bool IsChestPulseInProgress => m_PulseCoroutine != null;

    private void Awake()
    {
        if (m_PulseOrigin == null) {
            m_PulseOrigin = transform;
        }

        if (m_WeakPointsRoot != null) {
            m_WeakPointsRoot.SetActive(false);
        }
    }

    public override void OnPhaseEnter()
    {
        if (m_WeakPointsRoot != null) {
            m_WeakPointsRoot.SetActive(true);
        }
    }

    public override void OnPhaseExit()
    {
        CancelChestPulse();
        if (m_WeakPointsRoot != null) {
            m_WeakPointsRoot.SetActive(false);
        }
        base.OnPhaseExit();
    }

    public void PerformChestPulse()
    {
        if (m_BossCombat == null || !m_BossCombat.IsActivePhase<BossCombatPhase2>()) {
            return;
        }
        if (m_Health == null || !m_Health.IsAlive() || m_PulseCoroutine != null) {
            return;
        }

        m_PulseCoroutine = StartCoroutine(PulseCoroutine());
    }

    public void CancelChestPulse()
    {
        if (m_PulseCoroutine != null) {
            StopCoroutine(m_PulseCoroutine);
            m_PulseCoroutine = null;
        }
    }

    private IEnumerator PulseCoroutine()
    {
        yield return new WaitForSeconds(m_Windup);
        if (m_Health == null || !m_Health.IsAlive()) {
            m_PulseCoroutine = null;
            yield break;
        }

        SetWeakPointsActive(true);
        SpawnWave(m_Damage, m_MaxRadius, m_Speed);

        yield return new WaitForSeconds(m_VulnerabilityDuration);
        SetWeakPointsActive(false);
        m_PulseCoroutine = null;
    }

    private void SpawnWave(float damage, float maxRadius, float speed)
    {
        if (m_ShockwavePrefab == null) {
            return;
        }

        var origin = m_PulseOrigin != null ? m_PulseOrigin.position : transform.position;
        var wave = Instantiate(m_ShockwavePrefab, origin, Quaternion.identity);
        wave.Initialize(origin, gameObject, damage, maxRadius, speed);
    }
}
