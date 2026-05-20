using Opsive.UltimateCharacterController.Game;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class GiantHandSlamDamageApplier : MonoBehaviour
{
    [SerializeField] private BossCombatPhase _phase;
    [SerializeField] private GiantHandSlamDamage _handSlamDamage;
    [SerializeField] private float _damageAmount = 35f;
    [SerializeField] private float _force = 4f;
    [SerializeField] private LayerMask _playerDamageLayers = (1 << LayerManager.Character) | (1 << LayerManager.SubCharacter);

    public void ApplyHandSlamDamage()
    {
        if (!_phase.IsActive) {
            return;
        }

        _handSlamDamage.BeginSlam(_damageAmount, _force, gameObject, _playerDamageLayers);
    }
}
