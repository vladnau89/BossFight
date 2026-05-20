using NUnit.Framework;
using UnityEngine;

public class BossPhaseControllerComponentTests
{
    private GameObject _gameObject;
    private BossPhaseControllerComponent _controller;
    private TestBossCombatPhase _phase0;
    private TestBossCombatPhase _phase1;
    private bool _initialized;
    private int _phase0EnterCount;
    private int _phase0ExitCount;

    [SetUp]
    public void SetUp()
    {
        _gameObject = new GameObject("BossPhaseControllerComponentTests");
        _controller = _gameObject.AddComponent<BossPhaseControllerComponent>();
        _phase0 = _gameObject.AddComponent<TestBossCombatPhase>();
        _phase1 = _gameObject.AddComponent<TestBossCombatPhase>();

        TestReflectionHelper.SetField(_controller, "_phases", new BossCombatPhase[] { _phase0, _phase1 });
        TestReflectionHelper.SetField(_controller, "_startPhaseIndex", 0);

        _phase0.PhaseInitialized += () => _initialized = true;
        _phase0.PhaseEntered += () => _phase0EnterCount++;
        _phase0.PhaseExited += () => _phase0ExitCount++;
        _controller.Initialize();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_gameObject);
    }

    [Test]
    public void Initialize_AtStartPhaseIndex_CurrentPhaseShouldBePhase0()
    {
        Assert.That(_initialized, Is.True);
        Assert.That(_phase0EnterCount, Is.EqualTo(1));
        Assert.That(_controller.CurrentPhaseIndex, Is.EqualTo(0));
        Assert.That(_controller.CurrentPhase, Is.SameAs(_phase0));
    }

    [Test]
    public void EnterPhase_FromPhase0ToPhase1_CurrentPhaseShouldBePhase1()
    {
        _controller.EnterPhase(1);

        Assert.That(_phase0ExitCount, Is.EqualTo(1));
        Assert.That(_controller.CurrentPhaseIndex, Is.EqualTo(1));
        Assert.That(_controller.IsPhase2, Is.True);
        Assert.That(_phase0.ExitCount, Is.EqualTo(1));
        Assert.That(_phase1.EnterCount, Is.EqualTo(1));
    }

    private sealed class TestBossCombatPhase : BossCombatPhase
    {
        public int EnterCount { get; private set; }
        public int ExitCount { get; private set; }

        public override void OnPhaseEnter()
        {
            EnterCount++;
            base.OnPhaseEnter();
        }

        public override void OnPhaseExit()
        {
            ExitCount++;
            base.OnPhaseExit();
        }
    }
}
