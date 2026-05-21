using UnityEngine;

[DisallowMultipleComponent]
public sealed class GiantHandSlamMotionFinishPlayObserver_SpawnGroundShockwave : MonoBehaviour
{
    [SerializeField] private BossCombatPhase _phase;
    [SerializeField] private GiantHandSlamMotion _handSlamMotion;
    [SerializeField] private GroundShockwaveSpawner _groundShockwave;

    private void Awake() => _handSlamMotion.PlayFinished += OnPlayFinished;

    private void OnDestroy() => _handSlamMotion.PlayFinished -= OnPlayFinished;

    private void OnPlayFinished()
    {
        if (!_phase.IsActive) {
            return;
        }

        _groundShockwave.ScheduleSpawn();
    }
}
