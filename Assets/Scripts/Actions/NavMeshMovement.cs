using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    /// <summary>
    /// Base class for NavMesh movement tasks (compatible with Behavior Designer Movement Pack field names).
    /// </summary>
    public abstract class NavMeshMovement : Action
    {
        [Tooltip("The speed of the agent.")]
        public SharedFloat speed = 10f;

        [Tooltip("The angular speed of the agent.")]
        public SharedFloat angularSpeed = 120f;

        [Tooltip("The agent has arrived when the remaining distance is less than this value.")]
        public SharedFloat arriveDistance = 0.2f;

        [Tooltip("Should the agent stop when the task ends?")]
        public SharedBool stopOnTaskEnd = true;

        [Tooltip("Should the agent's rotation be updated?")]
        public SharedBool updateRotation = true;

        protected NavMeshAgent navMeshAgent;
        private bool m_StartedAgent;

        public override void OnAwake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        public override void OnStart()
        {
            if (navMeshAgent == null) {
                return;
            }

            navMeshAgent.speed = speed.Value;
            navMeshAgent.angularSpeed = angularSpeed.Value;
            navMeshAgent.updateRotation = updateRotation.Value;
            navMeshAgent.isStopped = false;

            if (!navMeshAgent.enabled) {
                navMeshAgent.enabled = true;
                m_StartedAgent = true;
            }

            SetDestination(GetDestination());
        }

        protected abstract Vector3 GetDestination();

        public override TaskStatus OnUpdate()
        {
            if (navMeshAgent == null || !navMeshAgent.enabled) {
                return TaskStatus.Failure;
            }

            var destination = GetDestination();
            if (Vector3.SqrMagnitude(navMeshAgent.destination - destination) > 0.01f) {
                SetDestination(destination);
            }

            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= arriveDistance.Value) {
                return TaskStatus.Success;
            }

            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            if (navMeshAgent == null) {
                return;
            }

            if (stopOnTaskEnd.Value) {
                navMeshAgent.isStopped = true;
                navMeshAgent.ResetPath();
            }

            if (m_StartedAgent) {
                navMeshAgent.enabled = false;
                m_StartedAgent = false;
            }
        }

        protected void SetDestination(Vector3 destination)
        {
            if (navMeshAgent == null || !navMeshAgent.isOnNavMesh) {
                return;
            }

            navMeshAgent.SetDestination(destination);
        }

        public override void OnReset()
        {
            speed = 10f;
            angularSpeed = 120f;
            arriveDistance = 0.2f;
            stopOnTaskEnd = true;
            updateRotation = true;
        }
    }
}
