using UnityEngine;

[DisallowMultipleComponent]
public sealed class BossChestPulseComponent : MonoBehaviour
{
    [SerializeField] private BossCombatPhase _phase;
    [SerializeField] private GroundShockwaveSpawner _groundShockwave;

    public bool IsChestPulseInProgress => _groundShockwave.IsBusy;

    public void CancelChestPulse() => _groundShockwave.Cancel();

    public void PerformChestPulse()
    {
        if (!_phase.IsActive) {
            return;
        }

        _groundShockwave.ScheduleSpawn();
    }
}
