using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.UltimateCharacterController
{
    [TaskCategory("Ultimate Character Controller")]
    [TaskDescription("Returns success when the boss has entered phase 2 (below health threshold).")]
    public class BossCombatIsPhase2 : Conditional
    {
        public SharedGameObject m_Boss;

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
            return m_Combat.IsPhase2 ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
