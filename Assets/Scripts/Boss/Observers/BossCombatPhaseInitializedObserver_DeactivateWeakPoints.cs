using UnityEngine;

[DisallowMultipleComponent]
public sealed class BossCombatPhaseInitializedObserver_DeactivateWeakPoints : MonoBehaviour
{
    [SerializeField] private BossCombatPhase _phase;
    [SerializeField] private BossPhaseWeakPointsComponent _weakPoints;

    private void Awake() => _phase.PhaseInitialized += OnPhaseInitialized;

    private void OnDestroy() => _phase.PhaseInitialized -= OnPhaseInitialized;

    private void OnPhaseInitialized() => _weakPoints.SetActive(false);
}
