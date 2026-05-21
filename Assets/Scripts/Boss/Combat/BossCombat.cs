using UnityEngine;

/// <summary>
/// Boss combat facade: phase transitions and BT entry points.
/// </summary>
[DefaultExecutionOrder(-100)]
public class BossCombat : MonoBehaviour
{
    [SerializeField] private BossPhaseControllerComponent _phaseController;
    [SerializeField] private BossCombatPhase1 _phase1;
    [SerializeField] private BossCombatPhase2 _phase2;

    public bool IsPhase2 => _phaseController.IsPhase2;
    public bool IsInProgress => _phaseController.InProgress;

    private void Start() => _phaseController.Initialize();

    public void ShowRangedPhase()
    {
        _phase1.ShowRanged();
        if (!IsPhase2) {
            _phase2.OnPhaseExit();
        }
    }

    public void ShowGiantHandPhase()
    {
        if (IsPhase2) {
            return;
        }

        _phase1.ShowGiantHandPrep();
        _phase2.OnPhaseExit();
    }

    public void PerformHandSlam(Transform target) => _phase1.PerformHandSlam(target);

    public void PerformChestPulse() => _phase2.PerformChestPulse();
}
