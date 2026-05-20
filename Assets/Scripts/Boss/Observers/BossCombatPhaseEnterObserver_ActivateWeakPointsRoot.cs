using UnityEngine;

[DisallowMultipleComponent]
public sealed class BossCombatPhaseEnterObserver_ActivateWeakPointsRoot : MonoBehaviour
{
    [SerializeField] private BossCombatPhase _phase;
    [SerializeField] private BossPhaseWeakPointsComponent _phaseWeakPoints;

    private void Awake() => _phase.PhaseEntered += OnPhaseEntered;

    private void OnDestroy() => _phase.PhaseEntered -= OnPhaseEntered;

    private void OnPhaseEntered() => _phaseWeakPoints.SetRootActive(true);
}
