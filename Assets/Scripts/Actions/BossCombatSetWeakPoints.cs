namespace BehaviorDesigner.Runtime.Tasks.UltimateCharacterController
{
    [TaskCategory("Ultimate Character Controller")]
    public class BossCombatSetWeakPoints : Action
    {
        public SharedGameObject m_Boss;
        public SharedBool m_Active = true;

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
            m_Combat.SetWeakPointsActive(m_Active.Value);
            return TaskStatus.Success;
        }
    }
}