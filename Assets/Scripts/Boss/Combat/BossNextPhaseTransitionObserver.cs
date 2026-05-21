using UnityEngine;

[DisallowMultipleComponent]
public sealed class BossNextPhaseTransitionObserver : MonoBehaviour
{
    [SerializeField] private BossPhaseControllerComponent _phaseController;
    [SerializeField] private BossPhaseEnterBinding[] _enterBindings;

    private void Update()
    {
        foreach (var binding in _enterBindings)
        {
            if (binding.PhaseIndex <= _phaseController.CurrentPhaseIndex)
            {
                continue;
            }

            if (binding.EnterCondition.ShouldEnter())
            {
                _phaseController.EnterPhase(binding.PhaseIndex);
                break;
            }
        }
    }
}