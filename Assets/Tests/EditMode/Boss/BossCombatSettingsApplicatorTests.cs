using System.Reflection;
using NUnit.Framework;
using Opsive.UltimateCharacterController.Traits;
using UnityEngine;

public class BossCombatSettingsApplicatorTests
{
    private GameObject _gameObject;
    private BossCombatSettingsApplicator _applicator;
    private BossCombatSettings _settings;
    private GroundShockwaveSpawner _phase1HandSlamShockwave;
    private BossPhaseHealthEnterCondition _phase2EnterCondition;
    private GiantHandSlamDamageApplier _phase1HandSlamDamage;
    private AttributeManager _bossAttributeManager;
    private BossPhaseWeakPointsComponent _phase1WeakPoints;
    private HealthComponent _weakPointHealth;
    private DamageValueComponent _weakPointDamage;

    [SetUp]
    public void SetUp()
    {
        _settings = ScriptableObject.CreateInstance<BossCombatSettings>();
        TestReflectionHelper.SetField(_settings, "_phase2EnterHealthFraction", 0.42f);
        TestReflectionHelper.SetField(_settings, "_bossMaxHealth", 1500f);
        object phase1 = BossCombatPhase1Settings.Default;
        TestReflectionHelper.SetField(phase1, "_handSlamShockwave", BossCombatShockwaveSettings.ChestPulseDefault);
        TestReflectionHelper.SetField(phase1, "_handSlamDamage", 99f);
        TestReflectionHelper.SetField(phase1, "_handSlamForce", 7f);
        TestReflectionHelper.SetField(phase1, "_weakPoints", new BossCombatWeakPointPhaseSettings(40f, 80f));
        TestReflectionHelper.SetField(_settings, "_phase1", phase1);

        _gameObject = new GameObject("BossCombatSettingsApplicatorTests");
        _applicator = _gameObject.AddComponent<BossCombatSettingsApplicator>();
        _phase1HandSlamShockwave = _gameObject.AddComponent<GroundShockwaveSpawner>();
        _phase2EnterCondition = _gameObject.AddComponent<BossPhaseHealthEnterCondition>();
        _phase1HandSlamDamage = _gameObject.AddComponent<GiantHandSlamDamageApplier>();
        _bossAttributeManager = _gameObject.AddComponent<AttributeManager>();

        var weakPointObject = new GameObject("WeakPoint");
        weakPointObject.transform.SetParent(_gameObject.transform);
        _weakPointHealth = weakPointObject.AddComponent<HealthComponent>();
        _weakPointDamage = weakPointObject.AddComponent<DamageValueComponent>();
        var weakPointEntity = weakPointObject.AddComponent<WeakPointEntity>();
        TestReflectionHelper.SetField(weakPointEntity, "_health", _weakPointHealth);
        TestReflectionHelper.SetField(weakPointEntity, "_damageValue", _weakPointDamage);

        _phase1WeakPoints = _gameObject.AddComponent<BossPhaseWeakPointsComponent>();
        TestReflectionHelper.SetField(_phase1WeakPoints, "_weakPoints", new[] { weakPointEntity });

        TestReflectionHelper.SetField(_applicator, "_settings", _settings);
        TestReflectionHelper.SetField(_applicator, "_phase2EnterCondition", _phase2EnterCondition);
        TestReflectionHelper.SetField(_applicator, "_phase1HandSlamShockwave", _phase1HandSlamShockwave);
        TestReflectionHelper.SetField(_applicator, "_phase1HandSlamDamage", _phase1HandSlamDamage);
        TestReflectionHelper.SetField(_applicator, "_behaviorTreeSettingsSync", (MonoBehaviour)null);
        TestReflectionHelper.SetField(_applicator, "_bossAttributeManager", _bossAttributeManager);
        TestReflectionHelper.SetField(_applicator, "_phase1WeakPoints", _phase1WeakPoints);

        var healthAttribute = new Attribute("Health", 100f);
        TestReflectionHelper.SetField(_bossAttributeManager, "m_Attributes", new[] { healthAttribute });
        typeof(AttributeManager).GetMethod("Initialize", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(_bossAttributeManager, null);
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

        Assert.That(TestReflectionHelper.GetField<float>(_phase1HandSlamShockwave, "_waveDelayMin"), Is.EqualTo(0.6f).Within(0.001f));
        Assert.That(TestReflectionHelper.GetField<float>(_phase1HandSlamShockwave, "_damage"), Is.EqualTo(18f).Within(0.001f));
        Assert.That(TestReflectionHelper.GetField<float>(_phase2EnterCondition, "_enterAtHealthFraction"), Is.EqualTo(0.42f).Within(0.001f));
        Assert.That(TestReflectionHelper.GetField<float>(_phase1HandSlamDamage, "_damageAmount"), Is.EqualTo(99f).Within(0.001f));
        Assert.That(TestReflectionHelper.GetField<float>(_phase1HandSlamDamage, "_force"), Is.EqualTo(7f).Within(0.001f));
        Assert.That(_bossAttributeManager.GetAttribute("Health").MaxValue, Is.EqualTo(1500f).Within(0.001f));
        Assert.That(_bossAttributeManager.GetAttribute("Health").Value, Is.EqualTo(1500f).Within(0.001f));
        Assert.That(TestReflectionHelper.GetField<float>(_weakPointHealth, "_maxHealth"), Is.EqualTo(40f).Within(0.001f));
        Assert.That(TestReflectionHelper.GetField<float>(_weakPointDamage, "Damage"), Is.EqualTo(80f).Within(0.001f));
    }
}
