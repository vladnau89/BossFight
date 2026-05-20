using NUnit.Framework;
using UnityEngine;

public class DestroyComponentTests
{
    private GameObject _gameObject;
    private DestroyComponent _destroy;
    private int _destroyEventCount;

    [SetUp]
    public void SetUp()
    {
        _gameObject = new GameObject("DestroyComponentTests");
        _destroy = _gameObject.AddComponent<DestroyComponent>();
        _destroyEventCount = 0;
        _destroy.EventDestroyed += OnDestroyed;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_gameObject);
    }

    [Test]
    public void ToDestroy_DestroyStateShouldBeSetAndEventRaised()
    {
        _destroy.ToDestroy();

        Assert.That(_destroy.IsDestroyed, Is.True);
        Assert.That(_destroyEventCount, Is.EqualTo(1));
    }

    [Test]
    public void ResetDestroy_AfterDestroy_DestroyedStateShouldBeCleared()
    {
        _destroy.ToDestroy();

        _destroy.ResetDestroy();

        Assert.That(_destroy.IsDestroyed, Is.False);
    }

    private void OnDestroyed() => _destroyEventCount++;
}
