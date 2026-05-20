using Opsive.UltimateCharacterController.Traits;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class BossDeathObserver_ResetToStartPhase : MonoBehaviour
{
    [SerializeField] private Health _health;
    [SerializeField] private BossPhaseControllerComponent _phaseController;

    private void Awake() => _health.OnDeathEvent.AddListener(OnDeath);

    private void OnDestroy() => _health.OnDeathEvent.RemoveListener(OnDeath);

    private void OnDeath(Vector3 position, Vector3 force, GameObject attacker) =>
        _phaseController.ResetToStartPhase();
}
