using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.UltimateCharacterController
{
    [TaskCategory("Ultimate Character Controller")]
    public class BossCombatHandSlam : Action
    {
        public SharedGameObject m_Boss;
        public SharedGameObject m_Target;

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
            if (m_Combat == null) {
                return TaskStatus.Failure;
            }

            if (!m_Started) {
                m_Started = true;
                Transform target = null;
                if (m_Target != null && m_Target.Value != null) {
                    target = m_Target.Value.transform;
                }
                m_Combat.PerformHandSlam(target);
            }

            if (m_Combat.IsHandSlamInProgress) {
                return TaskStatus.Running;
            }

            return TaskStatus.Success;
        }
    }
}