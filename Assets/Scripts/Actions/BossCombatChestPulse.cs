using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.UltimateCharacterController
{
    [TaskCategory("Ultimate Character Controller")]
    public class BossCombatChestPulse : Action
    {
        public SharedGameObject _boss;

        private BossCombat _combat;
        private bool _started;

        public override void OnStart()
        {
            var boss = GetDefaultGameObject(_boss.Value);
            _combat = boss != null ? boss.GetComponentInChildren<BossCombat>() : null;
            _started = false;
        }

        public override TaskStatus OnUpdate()
        {
            if (_combat == null || !_combat.IsPhase2) {
                return TaskStatus.Failure;
            }

            if (_started) {
                return TaskStatus.Success;
            }

            _started = true;
            _combat.ShowRangedPhase();
            _combat.PerformChestPulse();
            return TaskStatus.Success;
        }
    }
}
