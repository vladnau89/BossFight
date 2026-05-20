using UnityEngine;

[DisallowMultipleComponent]
public sealed class GroundShockwaveSpawnedObserver_ActivateWeakPoints : MonoBehaviour
{
    [SerializeField] private BossCombatPhase _phase;
    [SerializeField] private GroundShockwaveSpawner _groundShockwave;
    [SerializeField] private BossPhaseWeakPointsComponent _weakPoints;

    private void Awake() => _groundShockwave.SpawnScheduled += OnSpawnScheduled;

    private void OnDestroy() => _groundShockwave.SpawnScheduled -= OnSpawnScheduled;

    private void OnSpawnScheduled()
    {
        if (!_phase.IsActive) {
            return;
        }

        _weakPoints.SetActive(true);
    }
}
