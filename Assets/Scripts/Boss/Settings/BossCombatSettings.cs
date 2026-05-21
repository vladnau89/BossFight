using UnityEngine;

[CreateAssetMenu(fileName = "BossCombatSettings", menuName = "Boss/Boss Combat Settings")]
public sealed class BossCombatSettings : ScriptableObject
{
    [Header("Phase transition")]
    [SerializeField] [Range(0f, 1f)] private float _phase2EnterHealthFraction = 0.5f;

    [Header("Behavior tree (seconds)")]
    [SerializeField] private float _handSlamCooldown = 15f;
    [SerializeField] private float _chestPulseCooldown = 20f;

    [Header("Hand slam — ground shockwave")]
    [SerializeField] private BossCombatShockwaveSettings _handSlamShockwave = BossCombatShockwaveSettings.HandSlamDefault;

    [Header("Chest pulse — ground shockwave")]
    [SerializeField] private BossCombatShockwaveSettings _chestPulseShockwave = BossCombatShockwaveSettings.ChestPulseDefault;

    [Header("Hand slam — direct hit")]
    [SerializeField] private float _handSlamDamage = 35f;
    [SerializeField] private float _handSlamForce = 4f;

    public float Phase2EnterHealthFraction => _phase2EnterHealthFraction;
    public float HandSlamCooldown => _handSlamCooldown;
    public float ChestPulseCooldown => _chestPulseCooldown;
    public BossCombatShockwaveSettings HandSlamShockwave => _handSlamShockwave;
    public BossCombatShockwaveSettings ChestPulseShockwave => _chestPulseShockwave;
    public float HandSlamDamage => _handSlamDamage;
    public float HandSlamForce => _handSlamForce;

    /// <summary>Restores factory values (same as <c>BossCombatSettings_Default</c>).</summary>
    public void ResetToDefaults()
    {
        _phase2EnterHealthFraction = 0.5f;
        _handSlamCooldown = 15f;
        _chestPulseCooldown = 20f;
        _handSlamShockwave = BossCombatShockwaveSettings.HandSlamDefault;
        _chestPulseShockwave = BossCombatShockwaveSettings.ChestPulseDefault;
        _handSlamDamage = 35f;
        _handSlamForce = 4f;
    }
}
