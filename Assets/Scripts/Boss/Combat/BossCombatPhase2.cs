using UnityEngine;

/// <summary>
/// Phase 2 facade: chest pulse attack.
/// </summary>
[DisallowMultipleComponent]
public class BossCombatPhase2 : BossCombatPhase
{
    [SerializeField] private BossChestPulseComponent _chestPulse;

    public override bool InProgress => _chestPulse.IsChestPulseInProgress;

    public override void OnPhaseExit()
    {
        _chestPulse.CancelChestPulse();
        base.OnPhaseExit();
    }

    public void PerformChestPulse() => _chestPulse.PerformChestPulse();
}