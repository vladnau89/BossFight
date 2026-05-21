using UnityEngine;

[CreateAssetMenu(fileName = "BossCombatSettings", menuName = "Boss/Boss Combat Settings")]
public sealed class BossCombatSettings : ScriptableObject
{
    [Header("Global")]
    [SerializeField] [Range(0f, 1f)] private float _phase2EnterHealthFraction = 0.5f;
    [SerializeField] private float _bossMaxHealth = 2000f;
    [SerializeField] private float _bossRocketDamage = 1f;
    
    [Space(10)]
    [SerializeField] private BossCombatPhase1Settings _phase1 = BossCombatPhase1Settings.Default;

    [Space(10)]
    [SerializeField] private BossCombatPhase2Settings _phase2 = BossCombatPhase2Settings.Default;

    public float Phase2EnterHealthFraction => _phase2EnterHealthFraction;
    public float BossMaxHealth => _bossMaxHealth;
    public float BossRocketDamage => _bossRocketDamage;
    public BossCombatPhase1Settings Phase1 => _phase1;
    public BossCombatPhase2Settings Phase2 => _phase2;

    /// <summary>Restores factory values (same as <c>BossCombatSettings_Default</c>).</summary>
    public void ResetToDefaults()
    {
        _phase2EnterHealthFraction = 0.5f;
        _bossMaxHealth = 2000f;
        _bossRocketDamage = 1f;
        _phase1 = BossCombatPhase1Settings.Default;
        _phase2 = BossCombatPhase2Settings.Default;
    }
}
