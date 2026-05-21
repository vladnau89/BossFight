using UnityEngine;

[DisallowMultipleComponent]
public sealed class GroundShockwaveLogObserver : MonoBehaviour
{
    [SerializeField] private GroundShockwaveSpawner _groundShockwave;
    [SerializeField] private string _attackLabel = "Shockwave";

    private void OnEnable()
    {
        _groundShockwave.SpawnScheduled += OnSpawnScheduled;
        _groundShockwave.WaveSpawned += OnWaveSpawned;
    }

    private void OnDisable()
    {
        _groundShockwave.SpawnScheduled -= OnSpawnScheduled;
        _groundShockwave.WaveSpawned -= OnWaveSpawned;
    }

    private void OnSpawnScheduled()
    {
        BossCombatDebugLog.Log($"[Combat] {_attackLabel}: wave scheduled", _groundShockwave);
    }

    private void OnWaveSpawned()
    {
        BossCombatDebugLog.Log($"[Combat] {_attackLabel}: wave spawned", _groundShockwave);
    }
}
