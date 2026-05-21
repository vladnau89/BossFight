using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[DisallowMultipleComponent]
public sealed class GroundShockwaveSpawner : MonoBehaviour
{
    [SerializeField] private GroundShockwave _shockwavePrefab;
    [SerializeField] private Transform _spawnOrigin;
    [SerializeField] private BossCombatPhase _phase;
    [SerializeField] private float _waveDelayMin = 1f;
    [SerializeField] private float _waveDelayMax = 3f;
    [SerializeField] private float _damage = 12f;
    [SerializeField] private float _maxRadius = 20f;
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _holdAfterWaveSeconds;

    private Coroutine _pendingSpawn;
    private Coroutine _holdAfterWave;
    private GroundShockwave _activeWave;
    private bool _cancelling;

    public event Action SpawnScheduled;
    public event Action SpawnCancelled;
    public event Action WaveSpawned;
    public event Action WaveDestroyed;

    public bool IsBusy => _pendingSpawn != null || _activeWave != null || _holdAfterWave != null;

    public void ApplySettings(BossCombatShockwaveSettings settings)
    {
        _waveDelayMin = settings.WaveDelayMin;
        _waveDelayMax = settings.WaveDelayMax;
        _damage = settings.Damage;
        _maxRadius = settings.MaxRadius;
        _speed = settings.Speed;
        _holdAfterWaveSeconds = settings.HoldAfterWaveSeconds;
    }

    public void ScheduleSpawn()
    {
        ScheduleSpawn(Random.Range(_waveDelayMin, _waveDelayMax));
    }

    private void ScheduleSpawn(float delaySeconds)
    {
        CancelPending();
        SpawnScheduled?.Invoke();
        _pendingSpawn = StartCoroutine(SpawnDelayedCoroutine(delaySeconds));
    }

    public void CancelPending()
    {
        if (_pendingSpawn == null) {
            return;
        }

        StopCoroutine(_pendingSpawn);
        _pendingSpawn = null;
        SpawnCancelled?.Invoke();
    }

    public void CancelActiveWave()
    {
        if (_activeWave == null) {
            return;
        }

        Destroy(_activeWave.gameObject);
    }

    public void Cancel()
    {
        _cancelling = true;
        CancelPending();
        CancelHoldAfterWave(completeAttack: true);
        CancelActiveWave();
        _cancelling = false;
    }

    private IEnumerator SpawnDelayedCoroutine(float delaySeconds)
    {
        if (delaySeconds > 0f) {
            yield return new WaitForSeconds(delaySeconds);
        }

        _pendingSpawn = null;
        if (!_phase.IsActive) {
            SpawnCancelled?.Invoke();
            yield break;
        }

        SpawnWave();
    }

    private void SpawnWave()
    {
        var origin = _spawnOrigin.position;
        _activeWave = Instantiate(_shockwavePrefab, origin, Quaternion.identity);
        _activeWave.Initialize(origin, gameObject, _damage, _maxRadius, _speed, OnActiveWaveDestroyed);
        WaveSpawned?.Invoke();
    }

    private void OnActiveWaveDestroyed()
    {
        if (_activeWave == null) {
            return;
        }

        _activeWave = null;

        if (_cancelling || _holdAfterWaveSeconds <= 0f) {
            CompleteAttack();
            return;
        }

        CancelHoldAfterWave(completeAttack: false);
        _holdAfterWave = StartCoroutine(HoldAfterWaveCoroutine());
    }

    private IEnumerator HoldAfterWaveCoroutine()
    {
        yield return new WaitForSeconds(_holdAfterWaveSeconds);
        _holdAfterWave = null;
        WaveDestroyed?.Invoke();
    }

    private void CancelHoldAfterWave(bool completeAttack)
    {
        if (_holdAfterWave == null) {
            return;
        }

        StopCoroutine(_holdAfterWave);
        _holdAfterWave = null;

        if (completeAttack) {
            WaveDestroyed?.Invoke();
        }
    }

    private void CompleteAttack()
    {
        CancelHoldAfterWave(completeAttack: false);
        WaveDestroyed?.Invoke();
    }
}
