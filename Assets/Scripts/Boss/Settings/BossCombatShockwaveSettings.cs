using System;
using UnityEngine;

[Serializable]
public struct BossCombatShockwaveSettings
{
    [SerializeField] private float _waveDelayMin;
    [SerializeField] private float _waveDelayMax;
    [SerializeField] private float _damage;
    [SerializeField] private float _maxRadius;
    [SerializeField] private float _speed;
    [SerializeField] private float _holdAfterWaveSeconds;

    public float WaveDelayMin => _waveDelayMin;
    public float WaveDelayMax => _waveDelayMax;
    public float Damage => _damage;
    public float MaxRadius => _maxRadius;
    public float Speed => _speed;
    public float HoldAfterWaveSeconds => _holdAfterWaveSeconds;

    public static BossCombatShockwaveSettings HandSlamDefault => new BossCombatShockwaveSettings
    {
        _waveDelayMin = 1f,
        _waveDelayMax = 3f,
        _damage = 12f,
        _maxRadius = 20f,
        _speed = 10f,
        _holdAfterWaveSeconds = 0f,
    };

    public static BossCombatShockwaveSettings ChestPulseDefault => new BossCombatShockwaveSettings
    {
        _waveDelayMin = 0.6f,
        _waveDelayMax = 0.6f,
        _damage = 18f,
        _maxRadius = 20f,
        _speed = 10f,
        _holdAfterWaveSeconds = 3f,
    };
}
