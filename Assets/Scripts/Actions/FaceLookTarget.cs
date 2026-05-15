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
        public SharedGameObject m_TargetGameObject;

        [Tooltip("The object the character should face.")]
        public SharedGameObject m_LookTarget;

        [Tooltip("Wait until the body is facing the target before returning success.")]
        public SharedBool m_WaitUntilFacing = true;

        [Tooltip("Maximum angle (degrees) between body forward and target to count as facing.")]
        public SharedFloat m_ArrivalAngle = 5f;

        [Tooltip("Start the Aim ability so UCC rotates the body toward the look target.")]
        public SharedBool m_StartAimAbility = true;

        [Tooltip("Stop Aim and clear the look target when the task ends.")]
        public SharedBool m_StopOnEnd = true;

        [Tooltip("Return success even if Aim could not be started.")]
        public SharedBool m_AlwaysReturnSuccess;

        private GameObject m_PrevTarget;
        private UltimateCharacterLocomotion m_CharacterLocomotion;
        private LocalLookSource m_LocalLookSource;
        private Aim m_AimAbility;
        private bool m_StartedAim;

        public override void OnStart()
        {
            m_StartedAim = false;
            var target = GetDefaultGameObject(m_TargetGameObject.Value);
            if (target == m_PrevTarget) {
                return;
            }

            m_CharacterLocomotion = target.GetCachedComponent<UltimateCharacterLocomotion>();
            m_LocalLookSource = target.GetCachedComponent<LocalLookSource>();
            var aimAbilities = m_CharacterLocomotion != null
                ? m_CharacterLocomotion.GetAbilities<Aim>()
                : null;
            m_AimAbility = aimAbilities != null && aimAbilities.Length > 0 ? aimAbilities[0] : null;
            m_PrevTarget = target;
        }

        public override TaskStatus OnUpdate()
        {
            if (m_CharacterLocomotion == null || m_LocalLookSource == null) {
                return TaskStatus.Failure;
            }

            if (m_LookTarget.Value == null) {
                return TaskStatus.Failure;
            }

            m_LocalLookSource.Target = m_LookTarget.Value.transform;

            if (m_StartAimAbility.Value && m_AimAbility != null && !m_AimAbility.IsActive) {
                if (m_CharacterLocomotion.TryStartAbility(m_AimAbility)) {
                    m_StartedAim = true;
                } else if (!m_AlwaysReturnSuccess.Value) {
                    return TaskStatus.Failure;
                }
            }

            if (!m_WaitUntilFacing.Value) {
                return TaskStatus.Success;
            }

            return IsFacingTarget() ? TaskStatus.Success : TaskStatus.Running;
        }

        public override void OnEnd()
        {
            if (!m_StopOnEnd.Value || m_CharacterLocomotion == null) {
                return;
            }

            if (m_StartedAim && m_AimAbility != null && m_AimAbility.IsActive) {
                m_CharacterLocomotion.TryStopAbility(m_AimAbility);
            }

            if (m_LocalLookSource != null) {
                m_LocalLookSource.Target = null;
            }
        }

        private bool IsFacingTarget()
        {
            var direction = m_LookTarget.Value.transform.position - m_CharacterLocomotion.transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.0001f) {
                return true;
            }

            return Vector3.Angle(m_CharacterLocomotion.transform.forward, direction.normalized) <= m_ArrivalAngle.Value;
        }

        public override void OnReset()
        {
            m_TargetGameObject = null;
            m_LookTarget = null;
            m_WaitUntilFacing = true;
            m_ArrivalAngle = 5f;
            m_StartAimAbility = true;
            m_StopOnEnd = true;
            m_AlwaysReturnSuccess = false;
        }
    }
}
