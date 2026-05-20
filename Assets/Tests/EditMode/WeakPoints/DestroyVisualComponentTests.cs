using NUnit.Framework;
using UnityEngine;

public class DestroyVisualComponentTests
{
    private GameObject _gameObject;
    private DestroyVisualComponent _visual;

    [SetUp]
    public void SetUp()
    {
        _gameObject = new GameObject("DestroyVisualComponentTests");
        _visual = _gameObject.AddComponent<DestroyVisualComponent>();
        TestReflectionHelper.SetField(_visual, "_hitFlashDuration", 0.12f);
        TestReflectionHelper.SetField(_visual, "_hitFlashEmission", 6f);
        TestReflectionHelper.SetField(_visual, "_destroyFlashDuration", 0.25f);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_gameObject);
    }

    [Test]
    public void GetEmissionForDuration_ForHitDuration_EmissionShouldBeHitValue()
    {
        Assert.That(_visual.GetEmissionForDuration(0.12f), Is.EqualTo(6f));
    }

    [Test]
    public void GetEmissionForDuration_ForDestroyDuration_EmissionShouldBeDestroyValue()
    {
        Assert.That(_visual.GetEmissionForDuration(0.25f), Is.EqualTo(9f));
    }
}
