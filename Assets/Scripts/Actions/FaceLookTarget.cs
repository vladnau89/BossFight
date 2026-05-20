using Opsive.Shared.Game;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Character.Abilities.Items;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.UltimateCharacterController
{
    [TaskDescription("Sets the UCC look target and rotates the character body toward it (via LocalLookSource and Aim).")]
    [TaskCategory("Ultimate Character Controller")]
    [TaskIcon("Assets/Behavior Designer/Integrations/UltimateCharacterController/Editor/Icon.png")]
    public class FaceLookTarget : Action
    {
        [Tooltip("A reference to the agent. If null the task GameObject is used.")]
        public SharedGameObject _targetGameObject;

        [Tooltip("The object the character should face.")]
        public SharedGameObject _lookTarget;

        [Tooltip("Wait until the body is facing the target before returning success.")]
        public SharedBool _waitUntilFacing = true;

        [Tooltip("Maximum angle (degrees) between body forward and target to count as facing.")]
        public SharedFloat _arrivalAngle = 5f;

        [Tooltip("Start the Aim ability so UCC rotates the body toward the look target.")]
        public SharedBool _startAimAbility = true;

        [Tooltip("Stop Aim and clear the look target when the task ends.")]
        public SharedBool _stopOnEnd = true;

        [Tooltip("Return success even if Aim could not be started.")]
        public SharedBool _alwaysReturnSuccess;

        private GameObject _prevTarget;
        private UltimateCharacterLocomotion _characterLocomotion;
        private LocalLookSource _localLookSource;
        private Aim _aimAbility;
        private bool _startedAim;

        public override void OnStart()
        {
            _startedAim = false;
            var target = GetDefaultGameObject(_targetGameObject.Value);
            if (target == _prevTarget) {
                return;
            }

            _characterLocomotion = target.GetCachedComponent<UltimateCharacterLocomotion>();
            _localLookSource = target.GetCachedComponent<LocalLookSource>();
            var aimAbilities = _characterLocomotion != null
                ? _characterLocomotion.GetAbilities<Aim>()
                : null;
            _aimAbility = aimAbilities != null && aimAbilities.Length > 0 ? aimAbilities[0] : null;
            _prevTarget = target;
        }

        public override TaskStatus OnUpdate()
        {
            if (_characterLocomotion == null || _localLookSource == null) {
                return TaskStatus.Failure;
            }

            if (_lookTarget.Value == null) {
                return TaskStatus.Failure;
            }

            _localLookSource.Target = _lookTarget.Value.transform;

            if (_startAimAbility.Value && _aimAbility != null && !_aimAbility.IsActive) {
                if (_characterLocomotion.TryStartAbility(_aimAbility)) {
                    _startedAim = true;
                } else if (!_alwaysReturnSuccess.Value) {
                    return TaskStatus.Failure;
                }
            }

            if (!_waitUntilFacing.Value) {
                return TaskStatus.Success;
            }

            return IsFacingTarget() ? TaskStatus.Success : TaskStatus.Running;
        }

        public override void OnEnd()
        {
            if (!_stopOnEnd.Value || _characterLocomotion == null) {
                return;
            }

            if (_startedAim && _aimAbility != null && _aimAbility.IsActive) {
                _characterLocomotion.TryStopAbility(_aimAbility);
            }

            if (_localLookSource != null) {
                _localLookSource.Target = null;
            }
        }

        private bool IsFacingTarget()
        {
            var direction = _lookTarget.Value.transform.position - _characterLocomotion.transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.0001f) {
                return true;
            }

            return Vector3.Angle(_characterLocomotion.transform.forward, direction.normalized) <= _arrivalAngle.Value;
        }

        public override void OnReset()
        {
            _targetGameObject = null;
            _lookTarget = null;
            _waitUntilFacing = true;
            _arrivalAngle = 5f;
            _startAimAbility = true;
            _stopOnEnd = true;
            _alwaysReturnSuccess = false;
        }
    }
}
