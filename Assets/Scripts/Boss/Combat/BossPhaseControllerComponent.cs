using UnityEngine;

[DisallowMultipleComponent]
public sealed class BossPhaseControllerComponent : MonoBehaviour
{
    [SerializeField] private BossCombatPhase[] _phases;
    [SerializeField] private int _startPhaseIndex;
    [SerializeField] private BossCombatPhase1 _phase1;

    private int _currentPhaseIndex;

    public BossCombatPhase CurrentPhase => _phases[_currentPhaseIndex];
    public int CurrentPhaseIndex => _currentPhaseIndex;
    public bool IsPhase2 => _currentPhaseIndex == 1;
    public bool IsPhase1Active => CurrentPhase == _phase1;
    public bool InProgress => CurrentPhase.InProgress;

    public void Initialize()
    {
        _currentPhaseIndex = Mathf.Clamp(_startPhaseIndex, 0, _phases.Length - 1);

        foreach (var phase in _phases) {
            phase.Initialize();
        }

        CurrentPhase.OnPhaseEnter();
    }

    public void EnterPhase(int phaseIndex)
    {
        if (phaseIndex <= _currentPhaseIndex || phaseIndex >= _phases.Length) {
            return;
        }

        CurrentPhase.OnPhaseExit();

        _currentPhaseIndex = phaseIndex;
        CurrentPhase.OnPhaseEnter();

        _phase1.ShowRanged();
    }

    public void ExitAllPhases()
    {
        foreach (var phase in _phases) {
            phase.OnPhaseExit();
        }
    }

    public void EnterStartPhase()
    {
        _currentPhaseIndex = Mathf.Clamp(_startPhaseIndex, 0, _phases.Length - 1);
        CurrentPhase.OnPhaseEnter();
    }
}
