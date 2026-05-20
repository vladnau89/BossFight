namespace BehaviorDesigner.Runtime.Tasks.UltimateCharacterController
{
    [TaskCategory("Ultimate Character Controller")]
    public class BossCombatSetPhase : Action
    {
        public enum Phase
        {
            Ranged,
            GiantHand
        }

        public SharedGameObject _boss;
        public Phase _phase = Phase.Ranged;

        private BossCombat _combat;

        public override void OnStart()
        {
            var boss = GetDefaultGameObject(_boss.Value);
            _combat = boss != null ? boss.GetComponentInChildren<BossCombat>() : null;
        }

        public override TaskStatus OnUpdate()
        {
            if (_combat == null) {
                return TaskStatus.Failure;
            }
            if (_phase == Phase.GiantHand) {
                _combat.ShowGiantHandPhase();
            } else {
                _combat.ShowRangedPhase();
            }
            return TaskStatus.Success;
        }
    }
}
