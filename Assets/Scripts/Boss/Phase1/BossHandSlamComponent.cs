using UnityEngine;

[DisallowMultipleComponent]
public sealed class BossHandSlamComponent : MonoBehaviour
{
    [SerializeField] private BossCombatPhase _phase;
    [SerializeField] private BossPresentationComponent _presentation;
    [SerializeField] private GiantHandSlamMotion _handSlamMotion;
    [SerializeField] private GroundShockwaveSpawner _groundShockwave;

    public bool InProgress => _handSlamMotion.IsPlaying;

    public void CancelHandSlam()
    {
        _groundShockwave.CancelPending();
        _handSlamMotion.CancelAndRestore();
    }

    public void PerformHandSlam(Transform target)
    {
        if (!_phase.IsActive || InProgress) {
            return;
        }
        
        _handSlamMotion.Play(target, null);
    }
}
