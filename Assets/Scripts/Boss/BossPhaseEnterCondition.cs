using UnityEngine;

/// <summary>
/// Per-phase automatic transition condition evaluated by BossNextPhaseTransitionObserver.
/// </summary>
public abstract class BossPhaseEnterCondition : MonoBehaviour
{
    public abstract bool ShouldEnter();
}
