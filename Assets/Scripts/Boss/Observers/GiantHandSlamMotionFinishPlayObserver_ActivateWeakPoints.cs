using UnityEngine;

[DisallowMultipleComponent]
public sealed class GiantHandSlamMotionFinishPlayObserver_ActivateWeakPoints : MonoBehaviour
{
    [SerializeField] private BossCombatPhase _phase;
    [SerializeField] private GiantHandSlamMotion _handSlamMotion;
    [SerializeField] private BossPhaseWeakPointsComponent _weakPoints;

    private void Awake()
    {
        _handSlamMotion.PlayFinished += OnPlayFinished;
    }

    private void OnDestroy()
    {
        _handSlamMotion.PlayFinished -= OnPlayFinished;
    }

    private void OnPlayFinished()
    {
        if (!_phase.IsActive) {
            return;
        }

        _weakPoints.SetActive(true);
    }
}
