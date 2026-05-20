using System.Collections;
using Opsive.UltimateCharacterController.Traits;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class BossPhase2ChestPulseComponent : MonoBehaviour
{
    [SerializeField] private BossPhaseControllerComponent _phaseController;
    [SerializeField] private BossPhaseWeakPointsComponent _weakPoints;
    [SerializeField] private Transform _pulseOrigin;
    [SerializeField] private GroundShockwave _shockwavePrefab;
    [SerializeField] private float _windup = 0.6f;
    [SerializeField] private float _damage = 18f;
    [SerializeField] private float _maxRadius = 14f;
    [SerializeField] private float _speed = 12f;
    [SerializeField] private float _vulnerabilityDuration = 8f;
    [SerializeField] private Health _health;

    private Coroutine _pulseCoroutine;

    public bool IsChestPulseInProgress => _pulseCoroutine != null;

    public void CancelChestPulse()
    {
        if (_pulseCoroutine != null) {
            StopCoroutine(_pulseCoroutine);
            _pulseCoroutine = null;
        }
    }

    public void PerformChestPulse()
    {
        if (!_phaseController.IsPhase2 || !_health.IsAlive() || _pulseCoroutine != null) {
            return;
        }

        _pulseCoroutine = StartCoroutine(PulseCoroutine());
    }

    private IEnumerator PulseCoroutine()
    {
        yield return new WaitForSeconds(_windup);
        if (!_health.IsAlive()) {
            _pulseCoroutine = null;
            yield break;
        }

        _weakPoints.SetActive(true);
        SpawnWave(_damage, _maxRadius, _speed);

        yield return new WaitForSeconds(_vulnerabilityDuration);
        _weakPoints.SetActive(false);
        _pulseCoroutine = null;
    }

    private void SpawnWave(float damage, float maxRadius, float speed)
    {
        var origin = _pulseOrigin.position;
        var wave = Instantiate(_shockwavePrefab, origin, Quaternion.identity);
        wave.Initialize(origin, gameObject, damage, maxRadius, speed);
    }
}
