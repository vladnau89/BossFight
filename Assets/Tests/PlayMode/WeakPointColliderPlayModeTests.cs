using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class WeakPointColliderPlayModeTests
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
    public IEnumerator PrefabSpawn_ColliderActivationComponentsShouldBeWiredAndHitColliderEnabled()
    {
        _instance = Object.Instantiate(WeakPointPlayModeTestAssets.LoadWeakPointPrefab());
        yield return null;

        Assert.That(_instance.GetComponentInChildren<ColliderComponent>(true), Is.Not.Null);
        Assert.That(_instance.GetComponentInChildren<ColliderActivatorComponent>(true), Is.Not.Null);
        Assert.That(_instance.GetComponentInChildren<HealthResetObserver_ActivateCollider>(true), Is.Not.Null);
        Assert.That(_instance.GetComponentInChildren<DestroyObserver_DeactivateCollider>(true), Is.Not.Null);
        Assert.That(IsHitColliderEnabled(_instance), Is.True);
    }

    [UnityTest]
    public IEnumerator PrefabSpawn_HitColliderShouldBeEnabled()
    {
        _instance = Object.Instantiate(WeakPointPlayModeTestAssets.LoadWeakPointPrefab());
        yield return null;

        Assert.That(IsHitColliderEnabled(_instance), Is.True);
    }

    [UnityTest]
    public IEnumerator PartialDamage_EntityShouldBeNotDestroyedAndHitColliderEnabled()
    {
        _instance = Object.Instantiate(WeakPointPlayModeTestAssets.LoadWeakPointPrefab());
        yield return null;

        _instance.GetComponent<WeakPointEntity>().TakeDamage(10f);
        yield return null;

        Assert.That(_instance.GetComponent<WeakPointEntity>().IsDestroyed, Is.False);
        Assert.That(IsHitColliderEnabled(_instance), Is.True);
    }

    [UnityTest]
    public IEnumerator FullDamage_EntityShouldBeDestroyedAndHitColliderDisabled()
    {
        _instance = Object.Instantiate(WeakPointPlayModeTestAssets.LoadWeakPointPrefab());
        yield return null;

        _instance.GetComponent<WeakPointEntity>().TakeDamage(DefaultMaxHealth);
        yield return null;

        Assert.That(_instance.GetComponent<WeakPointEntity>().IsDestroyed, Is.True);
        Assert.That(IsHitColliderEnabled(_instance), Is.False);
    }

    [UnityTest]
    public IEnumerator ResetHealthAfterDestroy_EntityShouldBeNotDestroyedAndHitColliderEnabled()
    {
        _instance = Object.Instantiate(WeakPointPlayModeTestAssets.LoadWeakPointPrefab());
        yield return null;

        var entity = _instance.GetComponent<WeakPointEntity>();
        entity.TakeDamage(DefaultMaxHealth);
        yield return null;
        Assert.That(IsHitColliderEnabled(_instance), Is.False);

        entity.ResetHealth();
        yield return null;

        Assert.That(entity.IsDestroyed, Is.False);
        Assert.That(IsHitColliderEnabled(_instance), Is.True);
    }

    private static bool IsHitColliderEnabled(GameObject instance)
    {
        var colliderComponent = instance.GetComponentInChildren<ColliderComponent>(true);
        Assert.That(colliderComponent, Is.Not.Null);
        Assert.That(colliderComponent.Collider, Is.Not.Null);
        return colliderComponent.Collider.enabled;
    }
}
