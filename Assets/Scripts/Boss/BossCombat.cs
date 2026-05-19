using System;
using System.Collections.Generic;
using Opsive.UltimateCharacterController.Traits;
using UnityEngine;

/// <summary>
/// Boss combat coordinator: phase array, transitions, weak point damage routing, BT entry points.
/// </summary>
[DefaultExecutionOrder(-100)]
public class BossCombat : MonoBehaviour
{
    [Header("Phases")]
    [SerializeField] private BossCombatPhase[] m_Phases;
    [SerializeField] private int m_StartPhaseIndex;
    [SerializeField] private Health m_Health;
    
    private int m_CurrentPhaseIndex;
    private Dictionary<Collider, WeakPointMarker> m_WeakPointMarkersMap = new();
    
    public BossCombatPhase CurrentPhase => m_Phases[m_CurrentPhaseIndex];
    public bool IsPhase2 => m_CurrentPhaseIndex == 1;
    public bool IsInProgress => m_Phases[m_CurrentPhaseIndex].InProgress;

    public event Action<int> OnPhaseChanged;

    private void Awake()
    {
        m_CurrentPhaseIndex = Mathf.Clamp(m_StartPhaseIndex, 0, Mathf.Max(0, m_Phases.Length - 1));
        
        BuildWeakPointMap();

        foreach (var phase in m_Phases)
        {
            phase.SetWeakPointsActive(false);
        }

        
        CurrentPhase.OnPhaseEnter();

        if (m_Health != null) {
            m_Health.OnDeathEvent.AddListener(OnDeathEvent);
        }
    }

    private void OnDestroy()
    {
        if (m_Health != null) {
            m_Health.OnDeathEvent.RemoveListener(OnDeathEvent);
        }
    }

    private void Update()
    {
        if (m_Health == null || !m_Health.IsAlive() || m_Phases == null || m_Phases.Length == 0) {
            return;
        }

        var healthFraction = GetHealthFraction();
        for (var i = m_CurrentPhaseIndex + 1; i < m_Phases.Length; i++) {
            var phase = m_Phases[i];
            if (phase != null && phase.ShouldEnterAtHealth(healthFraction)) {
                EnterPhase(i);
                break;
            }
        }
    }
    
    public T GetPhase<T>() where T : BossCombatPhase
    {
        if (m_Phases == null) {
            return null;
        }

        foreach (var phase in m_Phases) {
            if (phase is T typedPhase) {
                return typedPhase;
            }
        }

        return null;
    }

    public bool IsActivePhase<T>() where T : BossCombatPhase => CurrentPhase is T;

    private void BuildWeakPointMap()
    {
        m_WeakPointMarkersMap.Clear();
        if (m_Phases == null) {
            return;
        }

        foreach (var phase in m_Phases) {
            phase?.RegisterWeakPoints(m_WeakPointMarkersMap);
        }
    }

    private float GetHealthFraction()
    {
        if (m_Health != null && m_Health.HealthMaxValue > 0f) {
            return m_Health.HealthValue / m_Health.HealthMaxValue;
        }

        return 1f;
    }

    public void EnterPhase(int phaseIndex)
    {
        if (m_Phases == null || phaseIndex <= m_CurrentPhaseIndex || phaseIndex >= m_Phases.Length) {
            return;
        }

        CurrentPhase?.OnPhaseExit();
        m_CurrentPhaseIndex = phaseIndex;
        CurrentPhase?.OnPhaseEnter();
        GetPhase<BossCombatPhase1>()?.ShowRanged();
        OnPhaseChanged?.Invoke(m_CurrentPhaseIndex);
    }
    
    private void OnDeathEvent(Vector3 arg0, Vector3 arg1, GameObject arg2)
    {
        foreach (var phase in m_Phases) {
            phase?.OnPhaseExit();
        }

        GetPhase<BossCombatPhase1>()?.ShowRanged();
        m_CurrentPhaseIndex = Mathf.Clamp(m_StartPhaseIndex, 0, Mathf.Max(0, m_Phases.Length - 1));
    }

    public void ShowRangedPhase()
    {
        GetPhase<BossCombatPhase1>()?.ShowRanged();
        if (!IsPhase2) {
            GetPhase<BossCombatPhase2>()?.OnPhaseExit();
        }
    }

    public void ShowGiantHandPhase()
    {
        if (IsPhase2) {
            return;
        }

        GetPhase<BossCombatPhase1>()?.ShowGiantHandPrep();
        GetPhase<BossCombatPhase2>()?.OnPhaseExit();
    }
    
    public bool IsWeakPointCollider(Collider hitCollider) => TryGetWeakPoint(hitCollider, out _);

    public bool TryDamageWeakPoint(Collider hitCollider, float amount, Vector3 position, Vector3 direction,
        float forceMagnitude, int frames, GameObject attacker, object attackerObject)
    {
        if (!TryGetWeakPoint(hitCollider, out var weakPoint)) {
            return false;
        }

        var wasDamaged = weakPoint.TakeDamage(amount);
        if (wasDamaged && weakPoint.IsDestroyed)
        {
            ApplyWeakPointBurstDamage(weakPoint.BossDamageOnDestroy, position, direction, attacker, attackerObject);
        }
        return wasDamaged;
    }

    private bool TryGetWeakPoint(Collider hitCollider, out WeakPointMarker weakPoint) =>
        m_WeakPointMarkersMap.TryGetValue(hitCollider, out weakPoint);

    private void ApplyWeakPointBurstDamage(float amount, Vector3 position, Vector3 direction, GameObject attacker,
        object attackerObject)
    {
        if (m_Health is BossCharacterHealth bossHealth) {
            bossHealth.ApplyWeakPointBurstDamage(amount, position, direction, attacker, attackerObject);
        }
    }
    
    public void PerformHandSlam(Transform target) => GetPhase<BossCombatPhase1>()?.PerformHandSlam(target);

    public void PerformChestPulse() => GetPhase<BossCombatPhase2>()?.PerformChestPulse();
}
