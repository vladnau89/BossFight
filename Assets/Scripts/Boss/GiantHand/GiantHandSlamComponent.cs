using UnityEngine;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
public sealed class BossHandSlamComponent : MonoBehaviour
{
    [SerializeField] private BossCombatPhase _phase;
    [FormerlySerializedAs("giantHandPresentation")] [SerializeField] private GiantHandSlamPresentationComponent giantHandSlamPresentation;
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
