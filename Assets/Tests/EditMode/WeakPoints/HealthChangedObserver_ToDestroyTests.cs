using NUnit.Framework;
using UnityEngine;

public class HealthChangedObserver_ToDestroyTests
{
    private GameObject _gameObject;
    private HealthComponent _health;
    private DestroyComponent _destroy;
    private HealthChangedObserver_ToDestroy _observer;

    [SetUp]
    public void SetUp()
    {
        _gameObject = new GameObject("HealthChangedObserver_ToDestroyTests");
        _health = _gameObject.AddComponent<HealthComponent>();
        _destroy = _gameObject.AddComponent<DestroyComponent>();
        _observer = _gameObject.AddComponent<HealthChangedObserver_ToDestroy>();

        TestReflectionHelper.SetField(_health, "_maxHealth", 30f);
        TestReflectionHelper.SetField(_observer, "_health", _health);
        TestReflectionHelper.SetField(_observer, "_destroy", _destroy);
        TestReflectionHelper.Invoke(_observer, "Subscribe");
        _health.ResetHealth();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_gameObject);
    }

    [Test]
    public void HealthDestroyed_DestroyComponentShouldBeMarkedDestroyed()
    {
        _health.ChangeHealth(-30f);

        Assert.That(_destroy.IsDestroyed, Is.True);
    }

    [Test]
    public void HealthReset_AfterDestroy_DestroyComponentShouldBeCleared()
    {
        _health.ChangeHealth(-30f);
        _health.ResetHealth();

        Assert.That(_destroy.IsDestroyed, Is.False);
    }
}
