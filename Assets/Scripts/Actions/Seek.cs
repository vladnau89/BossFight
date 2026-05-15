using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Seeks the specified target using the Unity NavMesh.")]
    [TaskCategory("Movement")]
    public class Seek : NavMeshMovement
    {
        [Tooltip("The GameObject that the agent is seeking.")]
        public SharedGameObject target;

        [Tooltip("If the target is null then use the target position.")]
        public SharedVector3 targetPosition;

        private bool m_DynamicTarget;

        protected override Vector3 GetDestination()
        {
            if (m_DynamicTarget && target.Value != null) {
                return target.Value.transform.position;
            }

            return targetPosition.Value;
        }

        public override void OnStart()
        {
            m_DynamicTarget = target != null && target.Value != null;
            base.OnStart();
        }

        public override void OnReset()
        {
            base.OnReset();
            target = null;
            targetPosition = Vector3.zero;
        }
    }
}
