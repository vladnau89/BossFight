using UnityEngine;

[DisallowMultipleComponent]
public sealed class GiantHandSlamMotionStartPlayObserver_DeactivateWeakPoints : MonoBehaviour
{
    [SerializeField] private BossCombatPhase _phase;
    [SerializeField] private GiantHandSlamMotion _handSlamMotion;
    [SerializeField] private BossPhaseWeakPointsComponent _weakPoints;

    private void Awake()
    {
        _handSlamMotion.PlayStarted += OnPlayStarted;
    }

    private void OnDestroy()
    {
        _handSlamMotion.PlayStarted -= OnPlayStarted;
    }

    private void OnPlayStarted()
    {
        if (!_phase.IsActive) {
            return;
        }

        _weakPoints.SetActive(false);
    }
}
