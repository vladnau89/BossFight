using UnityEngine;

[DisallowMultipleComponent]
public sealed class GiantHandSlamMotionFinishPlayObserver_ApplyHandSlamDamage : MonoBehaviour
{
    [SerializeField] private BossCombatPhase _phase;
    [SerializeField] private GiantHandSlamMotion _handSlamMotion;
    [SerializeField] private GiantHandSlamDamageApplier _handSlamDamageApplier;

    private void Awake() => _handSlamMotion.PlayFinished += OnPlayFinished;

    private void OnDestroy() => _handSlamMotion.PlayFinished -= OnPlayFinished;

    private void OnPlayFinished()
    {
        if (!_phase.IsActive) {
            return;
        }

        _handSlamDamageApplier.ApplyHandSlamDamage();
    }
}
