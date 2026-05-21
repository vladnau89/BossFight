using NUnit.Framework;
using UnityEngine;

public class BossCombatSettingsApplicatorTests
{
    private GameObject _gameObject;
    private BossCombatSettingsApplicator _applicator;
    private BossCombatSettings _settings;
    private GroundShockwaveSpawner _chestPulseShockwave;
    private BossPhaseHealthEnterCondition _phase2EnterCondition;
    private GiantHandSlamDamageApplier _handSlamDamageApplier;

    [SetUp]
    public void SetUp()
    {
        _settings = ScriptableObject.CreateInstance<BossCombatSettings>();
        TestReflectionHelper.SetField(_settings, "_phase2EnterHealthFraction", 0.42f);
        TestReflectionHelper.SetField<BossCombatShockwaveSettings>(_settings, "_chestPulseShockwave", BossCombatShockwaveSettings.ChestPulseDefault);
        TestReflectionHelper.SetField(_settings, "_handSlamDamage", 99f);
        TestReflectionHelper.SetField(_settings, "_handSlamForce", 7f);

        _gameObject = new GameObject("BossCombatSettingsApplicatorTests");
        _applicator = _gameObject.AddComponent<BossCombatSettingsApplicator>();
        _chestPulseShockwave = _gameObject.AddComponent<GroundShockwaveSpawner>();
        _phase2EnterCondition = _gameObject.AddComponent<BossPhaseHealthEnterCondition>();
        _handSlamDamageApplier = _gameObject.AddComponent<GiantHandSlamDamageApplier>();

        TestReflectionHelper.SetField(_applicator, "_settings", _settings);
        TestReflectionHelper.SetField(_applicator, "_phase2EnterCondition", _phase2EnterCondition);
        TestReflectionHelper.SetField(_applicator, "_handSlamShockwave", _chestPulseShockwave);
        TestReflectionHelper.SetField(_applicator, "_chestPulseShockwave", _chestPulseShockwave);
        TestReflectionHelper.SetField(_applicator, "_handSlamDamageApplier", _handSlamDamageApplier);
        TestReflectionHelper.SetField(_applicator, "_behaviorTreeSettingsSync", (MonoBehaviour)null);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_gameObject);
        Object.DestroyImmediate(_settings);
    }

    [Test]
    public void Apply_ShouldCopySettingsToCombatComponents()
    {
        _applicator.Apply();

        Assert.That(TestReflectionHelper.GetField<float>(_chestPulseShockwave, "_waveDelayMin"), Is.EqualTo(0.6f).Within(0.001f));
        Assert.That(TestReflectionHelper.GetField<float>(_chestPulseShockwave, "_damage"), Is.EqualTo(18f).Within(0.001f));
        Assert.That(TestReflectionHelper.GetField<float>(_phase2EnterCondition, "_enterAtHealthFraction"), Is.EqualTo(0.42f).Within(0.001f));
        Assert.That(TestReflectionHelper.GetField<float>(_handSlamDamageApplier, "_damageAmount"), Is.EqualTo(99f).Within(0.001f));
        Assert.That(TestReflectionHelper.GetField<float>(_handSlamDamageApplier, "_force"), Is.EqualTo(7f).Within(0.001f));
    }
}
