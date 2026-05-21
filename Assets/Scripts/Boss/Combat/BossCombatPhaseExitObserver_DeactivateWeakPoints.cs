using UnityEngine;

[DisallowMultipleComponent]
public sealed class BossCombatPhaseExitObserver_DeactivateWeakPoints : MonoBehaviour
{
    [SerializeField] private BossCombatPhase _phase;
    [SerializeField] private BossPhaseWeakPointsComponent _weakPoints;

    private void Awake() => _phase.PhaseExited += OnPhaseExited;

    private void OnDestroy() => _phase.PhaseExited -= OnPhaseExited;

    private void OnPhaseExited() => _weakPoints.SetActive(false);
}
