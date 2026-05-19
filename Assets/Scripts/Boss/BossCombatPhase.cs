using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shared weak point and lifecycle hooks for a boss combat phase.
/// </summary>
public abstract class BossCombatPhase : MonoBehaviour
{
    [Tooltip("Enter this phase when boss health fraction is at or below this value. Leave negative to disable auto-enter.")]
    [SerializeField] [Range(-1f, 1f)] protected float m_EnterAtHealthFraction = -1f;

    [SerializeField] protected GameObject m_WeakPointsRoot;
    [SerializeField] protected WeakPointMarker[] m_WeakPoints;

    public WeakPointMarker[] WeakPoints => m_WeakPoints;
    public float EnterAtHealthFraction => m_EnterAtHealthFraction;

    public bool HasHealthEnterThreshold => m_EnterAtHealthFraction >= 0f;

    public virtual bool InProgress { get; }

    public bool ShouldEnterAtHealth(float healthFraction)
    {
        return HasHealthEnterThreshold && healthFraction <= m_EnterAtHealthFraction;
    }
    
    public void RegisterWeakPoints(Dictionary<Collider, WeakPointMarker> map)
    {
        if (m_WeakPoints == null) {
            return;
        }

        foreach (var weakPoint in m_WeakPoints) {
            if (weakPoint != null && weakPoint.Collider != null) {
                map[weakPoint.Collider] = weakPoint;
            }
        }
    }

    public void SetWeakPointsActive(bool active)
    {
        if (m_WeakPointsRoot != null) {
            m_WeakPointsRoot.SetActive(active);
        }

        foreach (var weakPoint in m_WeakPoints) {
            if (weakPoint == null) {
                continue;
            }
            weakPoint.ResetHealth();
            weakPoint.SetActive(active);
        }
    }

    public virtual void OnPhaseEnter()
    {
    }

    public virtual void OnPhaseExit()
    {
        SetWeakPointsActive(false);
    }
}
