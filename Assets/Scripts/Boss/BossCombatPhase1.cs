using UnityEngine;

/// <summary>
/// Phase 1 facade: presentation + hand slam attack.
/// </summary>
[DisallowMultipleComponent]
public class BossCombatPhase1 : BossCombatPhase
{
    [SerializeField] private BossPhase1PresentationComponent _presentation;
    [SerializeField] private BossPhase1HandSlamComponent _handSlam;

    public override bool InProgress => _handSlam.InProgress;

    public override void OnPhaseEnter()
    {
        ShowRanged();
        base.OnPhaseEnter();
    }

    public override void OnPhaseExit()
    {
        _handSlam.CancelHandSlam();
        _presentation.HideGiantHand();
        base.OnPhaseExit();
    }

    public void ShowRanged() => _presentation.ShowRanged();

    public void ShowGiantHandPrep() => _presentation.ShowGiantHandPrep();

    public void PerformHandSlam(Transform target) => _handSlam.PerformHandSlam(target);
}
