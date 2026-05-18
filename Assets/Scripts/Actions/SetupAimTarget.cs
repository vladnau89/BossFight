using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.UltimateCharacterController
{
    [TaskCategory("Ultimate Character Controller")]
    [TaskDescription("Resolves AimPointComponent on the target and stores it in AimTarget and HomingTargetProvider on this agent.")]
    public sealed class SetupAimTarget : Action
    {
        [Tooltip("The detected enemy (usually the player root).")]
        public SharedGameObject _target;

        [Tooltip("The aim point on the enemy (for example chest).")]
        public SharedGameObject _aimTarget;

        public override TaskStatus OnUpdate()
        {
            Setup();
            return TaskStatus.Success;
        }

        private void Setup()
        {
            _aimTarget.Value = null;

            var homingProvider = gameObject.GetComponent<HomingTargetProvider>();
            if (homingProvider != null) {
                homingProvider.ClearHomingTarget();
            }

            if (_target == null || _target.Value == null) {
                return;
            }

            var aimPointComponent = _target.Value.GetComponentInParent<AimPointComponent>();
            if (aimPointComponent == null || aimPointComponent.AimPoint == null) {
                Debug.LogError($"AimPointComponent not found on {_target.Value}", _target.Value);
                return;
            }

            _aimTarget.Value = aimPointComponent.AimPoint.gameObject;

            if (homingProvider != null) {
                homingProvider.SetHomingTarget(aimPointComponent.AimPoint);
            }
        }

        public override void OnReset()
        {
            _aimTarget = null;
            _target = null;
        }
    }
}
