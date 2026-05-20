using Opsive.UltimateCharacterController.Game;
using UnityEngine;

/// <summary>
/// Damages characters that overlap the hand collider while a slam is active.
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class GiantHandSlamDamage : MonoBehaviour
{
    private static readonly Collider[] s_OverlapResults = new Collider[16];

    [SerializeField] private BoxCollider _collider;
    [SerializeField] private float _hitDuration = 0.65f;

    private bool _active;
    private float _damage;
    private float _forceMagnitude;
    private GameObject _attacker;
    private LayerMask _targetLayers = 1 << LayerManager.Character;
    private readonly System.Collections.Generic.HashSet<Opsive.UltimateCharacterController.Traits.Health> _hitThisSlam = new();

    private void Awake()
    {
        if (_collider == null) {
            _collider = GetComponent<BoxCollider>();
        }
        _collider.isTrigger = true;
        _collider.enabled = false;
    }

    public void BeginSlam(float damage, float forceMagnitude, GameObject attacker, LayerMask targetLayers)
    {
        _damage = damage;
        _forceMagnitude = forceMagnitude;
        _attacker = attacker;
        _targetLayers = targetLayers;
        _hitThisSlam.Clear();
        _active = true;
        _collider.enabled = true;
        ApplyOverlapDamage();
        CancelInvoke(nameof(EndSlam));
        Invoke(nameof(EndSlam), _hitDuration);
    }

    public void EndSlam()
    {
        _active = false;
        if (_collider != null) {
            _collider.enabled = false;
        }
    }

    private void FixedUpdate()
    {
        if (!_active) {
            return;
        }

        ApplyOverlapDamage();
    }

    private void ApplyOverlapDamage()
    {
        if (_collider == null || !_collider.enabled) {
            return;
        }

        var center = _collider.bounds.center;
        var halfExtents = _collider.bounds.extents;
        var count = Physics.OverlapBoxNonAlloc(center, halfExtents, s_OverlapResults, _collider.transform.rotation, _targetLayers, QueryTriggerInteraction.Collide);
        for (var i = 0; i < count; i++) {
            AreaDamageUtility.TryApplyDamage(s_OverlapResults[i], center, _damage, _forceMagnitude, _attacker, _targetLayers, _hitThisSlam, requireGrounded: false);
        }
    }

    private void OnDisable()
    {
        EndSlam();
    }
}
