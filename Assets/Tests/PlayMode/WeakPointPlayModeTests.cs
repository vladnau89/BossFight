using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class WeakPointPlayModeTests
{
    private const float DefaultMaxHealth = 30f;

    private GameObject _instance;

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        if (_instance != null) {
            Object.Destroy(_instance);
        }

        yield return null;
    }

    [UnityTest]
    public IEnumerator PrefabSpawn_ComponentsShouldBeWiredWithEntityHealthDestroyAndObserver()
    {
        _instance = Object.Instantiate(WeakPointPlayModeTestAssets.LoadWeakPointPrefab());
        yield return null;

        var entity = _instance.GetComponent<WeakPointEntity>();
        var health = _instance.GetComponentInChildren<HealthComponent>(true);
        var destroy = _instance.GetComponentInChildren<DestroyComponent>(true);
        var observer = _instance.GetComponentInChildren<HealthChangedObserver_ToDestroy>(true);

        Assert.That(entity, Is.Not.Null);
        Assert.That(health, Is.Not.Null);
        Assert.That(destroy, Is.Not.Null);
        Assert.That(observer, Is.Not.Null);
        Assert.That(entity.IsDestroyed, Is.False);
        Assert.That(destroy.IsDestroyed, Is.False);
    }

    [UnityTest]
    public IEnumerator TakeFullDamage_EntityShouldBeDestroyedViaLifecycleObserver()
    {
        _instance = Object.Instantiate(WeakPointPlayModeTestAssets.LoadWeakPointPrefab());
        yield return null;

        var entity = _instance.GetComponent<WeakPointEntity>();
        var destroy = _instance.GetComponentInChildren<DestroyComponent>(true);

        entity.TakeDamage(DefaultMaxHealth);
        yield return null;

        Assert.That(entity.IsDestroyed, Is.True);
        Assert.That(destroy.IsDestroyed, Is.True);
    }

    [UnityTest]
    public IEnumerator ResetHealthAfterFullDamage_DestroyShouldBeCleared()
    {
        _instance = Object.Instantiate(WeakPointPlayModeTestAssets.LoadWeakPointPrefab());
        yield return null;

        var entity = _instance.GetComponent<WeakPointEntity>();
        var destroy = _instance.GetComponentInChildren<DestroyComponent>(true);

        entity.TakeDamage(DefaultMaxHealth);
        yield return null;
        Assert.That(entity.IsDestroyed, Is.True);

        entity.ResetHealth();
        yield return null;

        Assert.That(entity.IsDestroyed, Is.False);
        Assert.That(destroy.IsDestroyed, Is.False);
    }
}
