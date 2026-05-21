using UnityEngine;

[DisallowMultipleComponent]
public sealed class GroundShockwaveDestroyedObserver_DeactivateWeakPoints : MonoBehaviour
{
    [SerializeField] private BossCombatPhase _phase;
    [SerializeField] private GroundShockwaveSpawner _groundShockwave;
    [SerializeField] private BossPhaseWeakPointsComponent _weakPoints;

    private void Awake()
    {
        _groundShockwave.WaveDestroyed += OnDeactivate;
        _groundShockwave.SpawnCancelled += OnDeactivate;
    }

    private void OnDestroy()
    {
        _groundShockwave.WaveDestroyed -= OnDeactivate;
        _groundShockwave.SpawnCancelled -= OnDeactivate;
    }

    private void OnDeactivate()
    {
        if (!_phase.IsActive) {
            return;
        }

        _weakPoints.SetActive(false);
    }
}
