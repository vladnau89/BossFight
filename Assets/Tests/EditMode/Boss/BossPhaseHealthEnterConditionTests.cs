using NUnit.Framework;
using UnityEngine;

public class BossPhaseHealthEnterConditionTests
{
    [Test]
    public void HealthEnterCondition_ShouldDeriveFromBossPhaseEnterCondition()
    {
        var gameObject = new GameObject("BossPhaseHealthEnterConditionTests");
        var enterCondition = gameObject.AddComponent<BossPhaseHealthEnterCondition>();

        Assert.That(enterCondition, Is.InstanceOf<BossPhaseEnterCondition>());

        Object.DestroyImmediate(gameObject);
    }
}
