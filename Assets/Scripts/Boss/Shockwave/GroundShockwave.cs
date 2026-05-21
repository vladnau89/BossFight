using System;
using System.Collections.Generic;
using Opsive.UltimateCharacterController.Game;
using Opsive.UltimateCharacterController.Traits;
using UnityEngine;

/// <summary>
/// Expanding ground damage ring. Spawned from <see cref="GroundShockwaveSpawner"/>.
/// </summary>
public class GroundShockwave : MonoBehaviour
{
    [SerializeField] private GroundShockwaveVisual _visual;
    [SerializeField] private float _speed = 12f;
    [SerializeField] private float _width = 2f;
    [SerializeField] private float _maxRadius = 25f;
    [SerializeField] private float _damage = 15f;
    [SerializeField] private float _forceMagnitude = 2f;
    [SerializeField] private LayerMask _targetLayers = (1 << LayerManager.Character) | (1 << LayerManager.SubCharacter);

    private float _radius;
    private GameObject _attacker;
    private Action _onDestroyed;
    private readonly HashSet<Health> _damagedTargets = new HashSet<Health>();

    public void Initialize(Vector3 position, GameObject attacker, float damage, float maxRadius, float speed, Action onDestroyed)
    {
        transform.position = position;
        _attacker = attacker;
        _damage = damage;
        _maxRadius = maxRadius;
        _speed = speed;
        _radius = _width;
        _onDestroyed = onDestroyed;
        _damagedTargets.Clear();
        _visual.UpdateRing(_radius);
    }

    private void Update()
    {
        var previousRadius = _radius;
        _radius += _speed * Time.deltaTime;
        if (_radius >= _maxRadius) {
            Destroy(gameObject);
            return;
        }

        AreaDamageUtility.DamageRing(transform.position, previousRadius, _radius, _damage, _forceMagnitude, _attacker, _targetLayers, _damagedTargets, requireGrounded: true);
        _visual.UpdateRing(_radius);
    }

    private void OnDestroy() => _onDestroyed?.Invoke();

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
