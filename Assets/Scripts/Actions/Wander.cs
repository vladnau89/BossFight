using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Wanders around the available NavMesh area.")]
    [TaskCategory("Movement")]
    public class Wander : NavMeshMovement
    {
        [Tooltip("The minimum distance to wander.")]
        public SharedFloat minWanderDistance = 20f;

        [Tooltip("The maximum distance to wander.")]
        public SharedFloat maxWanderDistance = 20f;

        [Tooltip("The amount that the agent rotates direction.")]
        public SharedFloat wanderRate = 2f;

        [Tooltip("The minimum pause duration at each wander destination.")]
        public SharedFloat minPauseDuration;

        [Tooltip("The maximum pause duration at each wander destination.")]
        public SharedFloat maxPauseDuration;

        [Tooltip("The number of retries before the agent gives up.")]
        public SharedInt targetRetries = 1;

        private Vector3 m_Destination;
        private float m_PauseTime = -1f;

        protected override Vector3 GetDestination()
        {
            return m_Destination;
        }

        public override void OnStart()
        {
            m_PauseTime = -1f;
            if (!TrySetNewDestination()) {
                m_Destination = transform.position;
            }

            base.OnStart();
        }

        public override TaskStatus OnUpdate()
        {
            if (m_PauseTime > 0f) {
                m_PauseTime -= Time.deltaTime;
                return TaskStatus.Running;
            }

            if (navMeshAgent == null || !navMeshAgent.enabled) {
                return TaskStatus.Failure;
            }

            if (m_PauseTime <= 0f && !navMeshAgent.pathPending &&
                navMeshAgent.remainingDistance <= arriveDistance.Value) {
                if (!TrySetNewDestination()) {
                    return TaskStatus.Running;
                }

                if (maxPauseDuration.Value > 0f || minPauseDuration.Value > 0f) {
                    m_PauseTime = Random.Range(minPauseDuration.Value, maxPauseDuration.Value);
                }

                SetDestination(m_Destination);
            }

            return TaskStatus.Running;
        }

        private bool TrySetNewDestination()
        {
            var retries = Mathf.Max(1, targetRetries.Value);
            for (var i = 0; i < retries; ++i) {
                var direction = transform.forward + Random.insideUnitSphere * wanderRate.Value;
                direction.y = 0f;
                if (direction.sqrMagnitude < 0.01f) {
                    direction = transform.forward;
                }

                var distance = Random.Range(minWanderDistance.Value, maxWanderDistance.Value);
                var targetPosition = transform.position + direction.normalized * distance;

                if (NavMesh.SamplePosition(targetPosition, out var hit, maxWanderDistance.Value, NavMesh.AllAreas)) {
                    m_Destination = hit.position;
                    return true;
                }
            }

            return false;
        }

        public override void OnReset()
        {
            base.OnReset();
            minWanderDistance = 20f;
            maxWanderDistance = 20f;
            wanderRate = 2f;
            minPauseDuration = 0f;
            maxPauseDuration = 0f;
            targetRetries = 1;
        }
    }
}
