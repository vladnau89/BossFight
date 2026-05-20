using NUnit.Framework;
using UnityEngine;

public class HealthComponentTests
{
    private GameObject _gameObject;
    private HealthComponent _health;
    private HealthChangeEventData _lastEvent;

    [SetUp]
    public void SetUp()
    {
        _gameObject = new GameObject("HealthComponentTests");
        _health = _gameObject.AddComponent<HealthComponent>();
        TestReflectionHelper.SetField(_health, "_maxHealth", 30f);
        _health.ResetHealth();
        _health.EventHealthChanged += data => _lastEvent = data;
        _lastEvent = default;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_gameObject);
    }

    [Test]
    public void ResetHealth_AfterDamageTaken_HealthEventShouldBeResetWithFullHealth()
    {
        _health.ChangeHealth(-10f);

        _health.ResetHealth();

        Assert.That(_lastEvent.WasReset, Is.True);
        Assert.That(_lastEvent.CurrentHealth, Is.EqualTo(30f));
    }

    [Test]
    public void ChangeHealth_WhileAlive_HealthEventShouldBeHitWithReducedHealth()
    {
        _health.ChangeHealth(-10f);

        Assert.That(_lastEvent.WasHit, Is.True);
        Assert.That(_lastEvent.CurrentHealth, Is.EqualTo(20f));
    }

    [Test]
    public void ChangeHealth_AtZeroHealth_HealthEventShouldBeDestroyed()
    {
        _health.ChangeHealth(-30f);

        Assert.That(_lastEvent.WasDestroyed, Is.True);
        Assert.That(_lastEvent.CurrentHealth, Is.EqualTo(0f));
    }

    [Test]
    public void ChangeHealth_AboveMax_CurrentHealthShouldBeClampedToMax()
    {
        _health.ChangeHealth(100f);

        Assert.That(_lastEvent.CurrentHealth, Is.EqualTo(30f));
    }
}
