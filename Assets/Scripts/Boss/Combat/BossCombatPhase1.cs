using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Phase 1 facade: presentation + hand slam attack.
/// </summary>
[DisallowMultipleComponent]
public class BossCombatPhase1 : BossCombatPhase
{
    [FormerlySerializedAs("_presentation")]
    [SerializeField] private GiantHandSlamPresentationComponent giantHandSlamPresentation;
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
        giantHandSlamPresentation.HideGiantHand();
        base.OnPhaseExit();
    }

    public void ShowRanged() => giantHandSlamPresentation.ShowRanged();

    public void ShowGiantHandPrep() => giantHandSlamPresentation.ShowGiantHandPrep();

    public void PerformHandSlam(Transform target)
    {
        if (!IsActive || InProgress) {
            return;
        }

        giantHandSlamPresentation.ShowGiantHand();
        _handSlamMotion.Play(target, null);
    }
}
