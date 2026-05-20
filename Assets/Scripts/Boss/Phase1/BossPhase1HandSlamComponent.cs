using System.Collections;
using Opsive.UltimateCharacterController.Game;
using Opsive.UltimateCharacterController.Traits;
using UnityEngine;
using Random = UnityEngine.Random;

[DisallowMultipleComponent]
public sealed class BossPhase1HandSlamComponent : MonoBehaviour
{
    [SerializeField] private BossCombatPhase _phase;
    [SerializeField] private BossPhase1PresentationComponent _presentation;
    [SerializeField] private BossPhaseWeakPointsComponent _weakPoints;
    [SerializeField] private GiantHandSlamMotion _handSlamMotion;
    [SerializeField] private GiantHandSlamDamage _handSlamDamage;
    [SerializeField] private GroundShockwave _shockwavePrefab;
    [SerializeField] private Transform _slamOrigin;
    [SerializeField] private float _handSlamDamageAmount = 35f;
    [SerializeField] private float _handSlamForce = 4f;
    [SerializeField] private float _waveDelayMin = 1f;
    [SerializeField] private float _waveDelayMax = 3f;
    [SerializeField] private float _waveDamage = 12f;
    [SerializeField] private float _waveMaxRadius = 20f;
    [SerializeField] private float _waveSpeed = 10f;
    [SerializeField] private LayerMask _playerDamageLayers = (1 << LayerManager.Character) | (1 << LayerManager.SubCharacter);
    [SerializeField] private Health _health;

    public bool InProgress => _handSlamMotion.IsPlaying;

    public void CancelHandSlam()
    {
        StopAllCoroutines();
        _handSlamMotion.CancelAndRestore();
    }

    public void PerformHandSlam(Transform target)
    {
        if (!_phase.IsActive || InProgress || target == null) {
            return;
        }

        _weakPoints.SetActive(false);
        _presentation.GiantHandRoot.SetActive(true);
        _handSlamMotion.Play(target, OnHandSlamLanded);
    }

    private void OnHandSlamLanded()
    {
        if (!_phase.IsActive) {
            return;
        }

        _weakPoints.SetActive(true);
        ApplyHandSlamDamage();
        StartCoroutine(SpawnGroundShockwaveCoroutine());
    }

    private void ApplyHandSlamDamage()
    {
        if (!_health.IsAlive()) {
            return;
        }

        _handSlamDamage.BeginSlam(_handSlamDamageAmount, _handSlamForce, gameObject, _playerDamageLayers);
    }

    private IEnumerator SpawnGroundShockwaveCoroutine()
    {
        yield return new WaitForSeconds(Random.Range(_waveDelayMin, _waveDelayMax));
        if (!_health.IsAlive() || !_phase.IsActive) {
            yield break;
        }

        SpawnWave(_waveDamage, _waveMaxRadius, _waveSpeed);
    }

    private void SpawnWave(float damage, float maxRadius, float speed)
    {
        var origin = _slamOrigin.position;
        var wave = Instantiate(_shockwavePrefab, origin, Quaternion.identity);
        wave.Initialize(origin, gameObject, damage, maxRadius, speed);
    }
}
