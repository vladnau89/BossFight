using System;
using UnityEngine;

/// <summary>
/// Boss combat phase: lifecycle hooks and phase events.
/// </summary>
public abstract class BossCombatPhase : MonoBehaviour
{
    public event Action PhaseInitialized;
    public event Action PhaseExited;
    public event Action PhaseEntered;

    public bool IsActive { get; private set; }
    public virtual bool InProgress => false;

    public void Initialize() => PhaseInitialized?.Invoke();

    public virtual void OnPhaseEnter()
    {
        IsActive = true;
        PhaseEntered?.Invoke();
    }

    public virtual void OnPhaseExit()
    {
        IsActive = false;
        PhaseExited?.Invoke();
    }
}
