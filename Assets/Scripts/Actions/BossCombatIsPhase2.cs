using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.UltimateCharacterController
{
    [TaskCategory("Ultimate Character Controller")]
    public class BossCombatIsPhase2 : Conditional
    {
        public SharedGameObject _boss;

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
            return _combat.IsPhase2 ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
