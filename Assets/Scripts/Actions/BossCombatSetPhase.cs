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

        public SharedGameObject m_Boss;
        public Phase m_Phase = Phase.Ranged;

        private BossCombat m_Combat;

        public override void OnStart()
        {
            var boss = GetDefaultGameObject(m_Boss.Value);
            m_Combat = boss != null ? boss.GetComponent<BossCombat>() : null;
        }

        public override TaskStatus OnUpdate()
        {
            if (m_Combat == null) {
                return TaskStatus.Failure;
            }
            if (m_Phase == Phase.GiantHand) {
                m_Combat.ShowGiantHandPhase();
            } else {
                m_Combat.ShowRangedPhase();
            }
            return TaskStatus.Success;
        }
    }
}
