namespace BehaviorDesigner.Runtime.Tasks.UltimateCharacterController
{
    [TaskCategory("Ultimate Character Controller")]
    [TaskDescription("Phase 2: shock pulse from the boss and expose chest weak points.")]
    public class BossCombatChestPulse : Action
    {
        public SharedGameObject m_Boss;

        private BossCombat m_Combat;
        private bool m_Started;

        public override void OnStart()
        {
            var boss = GetDefaultGameObject(m_Boss.Value);
            m_Combat = boss != null ? boss.GetComponent<BossCombat>() : null;
            m_Started = false;
        }

        public override TaskStatus OnUpdate()
        {
            if (m_Combat == null || !m_Combat.IsPhase2) {
                return TaskStatus.Failure;
            }

            if (!m_Started) {
                m_Started = true;
                m_Combat.PerformChestPulse();
            }

            if (m_Combat.IsChestPulseInProgress) {
                return TaskStatus.Running;
            }

            return TaskStatus.Success;
        }
    }
}