using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.UltimateCharacterController
{
    [TaskCategory("Ultimate Character Controller")]
    public class BossCombatHandSlam : Action
    {
        public SharedGameObject _boss;
        public SharedGameObject _target;

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
            if (_combat == null) {
                return TaskStatus.Failure;
            }

            if (!_started) {
                _started = true;
                Transform target = null;
                if (_target != null && _target.Value != null) {
                    target = _target.Value.transform;
                }
                _combat.PerformHandSlam(target);
            }

            if (_combat.IsInProgress) {
                return TaskStatus.Running;
            }

            return TaskStatus.Success;
        }
    }
}
