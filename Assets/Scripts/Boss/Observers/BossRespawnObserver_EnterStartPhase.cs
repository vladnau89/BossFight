using Opsive.Shared.Events;
using Opsive.UltimateCharacterController.Traits;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class BossRespawnObserver_EnterStartPhase : MonoBehaviour
{
    [SerializeField] private Health _health;
    [SerializeField] private BossPhaseControllerComponent _phaseController;

    private void Awake() => EventHandler.RegisterEvent(_health.gameObject, "OnRespawn", OnRespawn);

    private void OnDestroy() => EventHandler.UnregisterEvent(_health.gameObject, "OnRespawn", OnRespawn);

    private void OnRespawn() => _phaseController.EnterStartPhase();
}
