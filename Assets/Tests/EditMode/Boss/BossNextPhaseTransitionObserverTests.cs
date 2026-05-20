using NUnit.Framework;
using UnityEngine;

public class BossNextPhaseTransitionObserverTests
{
    private GameObject _gameObject;
    private BossPhaseControllerComponent _controller;
    private BossNextPhaseTransitionObserver _observer;
    private TestBossCombatPhase _phase0;
    private BossCombatPhase1 _phase1;
    private TestPhaseEnterCondition _phase1EnterCondition;

    [SetUp]
    public void SetUp()
    {
        _gameObject = new GameObject("BossNextPhaseTransitionObserverTests");
        _controller = _gameObject.AddComponent<BossPhaseControllerComponent>();
        _observer = _gameObject.AddComponent<BossNextPhaseTransitionObserver>();
        _phase0 = _gameObject.AddComponent<TestBossCombatPhase>();
        _phase1 = _gameObject.AddComponent<BossCombatPhase1>();
        _phase1EnterCondition = _gameObject.AddComponent<TestPhaseEnterCondition>();

        var presentation = _gameObject.AddComponent<BossPresentationComponent>();
        var rangedRoot = new GameObject("Ranged");
        var handRoot = new GameObject("Hand");
        TestReflectionHelper.SetField(presentation, "_rangedWeaponRoot", rangedRoot);
        TestReflectionHelper.SetField(presentation, "_giantHandRoot", handRoot);
        TestReflectionHelper.SetField(_phase1, "_presentation", presentation);

        TestReflectionHelper.SetField(_controller, "_phases", new BossCombatPhase[] { _phase0, _phase1 });
        TestReflectionHelper.SetField(_controller, "_startPhaseIndex", 0);
        TestReflectionHelper.SetField(_controller, "_phase1", _phase1);

        var bindings = new BossPhaseEnterBinding[1];
        TestReflectionHelper.SetField(bindings[0], "_phaseIndex", 1);
        TestReflectionHelper.SetField(bindings[0], "_enterCondition", _phase1EnterCondition);
        TestReflectionHelper.SetField(_observer, "_phaseController", _controller);
        TestReflectionHelper.SetField(_observer, "_enterBindings", bindings);

        _controller.Initialize();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_gameObject);
    }

    [Test]
    public void Update_WhenEnterConditionMet_ShouldEnterPhase1()
    {
        _phase1EnterCondition.ShouldEnterResult = true;

        _observer.SendMessage("Update");

        Assert.That(_controller.CurrentPhaseIndex, Is.EqualTo(1));
    }

    private sealed class TestBossCombatPhase : BossCombatPhase
    {
    }

    private sealed class TestPhaseEnterCondition : BossPhaseEnterCondition
    {
        public bool ShouldEnterResult;

        public override bool ShouldEnter() => ShouldEnterResult;
    }
}
