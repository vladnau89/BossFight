using UnityEngine;

/// <summary>
/// Phase 1 facade: presentation + hand slam attack.
/// </summary>
[DisallowMultipleComponent]
public class BossCombatPhase1 : BossCombatPhase
{
    [SerializeField] private BossPresentationComponent _presentation;
    [SerializeField] private GiantHandSlamMotion _handSlamMotion;
    [SerializeField] private GroundShockwaveSpawner _groundShockwave;

    public override bool InProgress => _handSlamMotion.IsPlaying;

    public override void OnPhaseEnter()
    {
        ShowRanged();
        base.OnPhaseEnter();
    }

    public override void OnPhaseExit()
    {
        _groundShockwave.CancelPending();
        _handSlamMotion.CancelAndRestore();
        _presentation.HideGiantHand();
        base.OnPhaseExit();
    }

    public void ShowRanged() => _presentation.ShowRanged();

    public void ShowGiantHandPrep() => _presentation.ShowGiantHandPrep();

    public void PerformHandSlam(Transform target)
    {
        if (!IsActive || InProgress) {
            return;
        }

        _presentation.ShowGiantHand();
        _handSlamMotion.Play(target, null);
    }
}
