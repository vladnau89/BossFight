using NUnit.Framework;

public class HealthChangeEventDataTests
{
    [Test]
    public void HealthDecreaseAboveZero_EventFlagsShouldBeHitOnly()
    {
        var data = new HealthChangeEventData(30f, 20f, 30f, wasReset: false);

        Assert.That(data.WasHit, Is.True);
        Assert.That(data.WasDestroyed, Is.False);
        Assert.That(data.WasReset, Is.False);
    }

    [Test]
    public void HealthAtZero_EventFlagsShouldBeDestroyedOnly()
    {
        var data = new HealthChangeEventData(10f, 0f, 30f, wasReset: false);

        Assert.That(data.WasDestroyed, Is.True);
        Assert.That(data.WasHit, Is.False);
    }

    [Test]
    public void ResetEvent_EventFlagsShouldBeResetOnly()
    {
        var data = new HealthChangeEventData(0f, 30f, 30f, wasReset: true);

        Assert.That(data.WasReset, Is.True);
        Assert.That(data.WasHit, Is.False);
        Assert.That(data.WasDestroyed, Is.False);
    }
}
