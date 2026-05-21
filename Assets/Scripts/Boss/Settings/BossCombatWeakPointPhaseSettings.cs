using System;
using UnityEngine;

[Serializable]
public struct BossCombatWeakPointPhaseSettings
{
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _bossBurstDamage;

    public float MaxHealth => _maxHealth;
    public float BossBurstDamage => _bossBurstDamage;

    public BossCombatWeakPointPhaseSettings(float maxHealth, float bossBurstDamage)
    {
        _maxHealth = maxHealth;
        _bossBurstDamage = bossBurstDamage;
    }

    public static BossCombatWeakPointPhaseSettings Default => new BossCombatWeakPointPhaseSettings
    {
        _maxHealth = 30f,
        _bossBurstDamage = 100f,
    };
}
