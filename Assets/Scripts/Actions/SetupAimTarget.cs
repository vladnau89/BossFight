using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.UltimateCharacterController
{
    [TaskCategory("Ultimate Character Controller")]
    public sealed class SetupAimTarget : Action
    {
        public SharedGameObject _target;
        public SharedGameObject _aimTarget;

        public override TaskStatus OnUpdate()
        {
            Setup();
            return TaskStatus.Success;
        }

        private void Setup()
        {
            _aimTarget.Value = null;
            
            if (_target == null || _target.Value == null)
            {
                return;
            }

            var aimPointComponent = _target.Value.GetComponentInParent<AimPointComponent>();
            if (aimPointComponent == null)
            {
                Debug.LogError($"Not found AimPointComponent on {_target.Value}", _target.Value);
                return;
            }

            _aimTarget.Value = aimPointComponent.AimPoint.gameObject;
        }
        
        public override void OnReset()
        {
            _aimTarget = null;
            _target = null;
        }
    }
}