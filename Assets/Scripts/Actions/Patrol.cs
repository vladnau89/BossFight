using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Patrols around the specified waypoints using the Unity NavMesh.")]
    [TaskCategory("Movement")]
    public class Patrol : NavMeshMovement
    {
        [Tooltip("Should the agent patrol the waypoints randomly?")]
        public SharedBool randomPatrol;

        [Tooltip("The amount of time to pause at each waypoint.")]
        public SharedFloat waypointPauseDuration;

        [Tooltip("The waypoints to patrol.")]
        public SharedGameObjectList waypoints;

        private int m_WaypointIndex;
        private float m_PauseTime = -1f;

        protected override Vector3 GetDestination()
        {
            if (waypoints == null || waypoints.Value == null || waypoints.Value.Count == 0) {
                return transform.position;
            }

            var waypoint = waypoints.Value[m_WaypointIndex];
            return waypoint != null ? waypoint.transform.position : transform.position;
        }

        public override void OnStart()
        {
            m_PauseTime = -1f;
            m_WaypointIndex = 0;

            if (waypoints != null && waypoints.Value != null && waypoints.Value.Count > 0) {
                var distance = float.MaxValue;
                for (var i = 0; i < waypoints.Value.Count; ++i) {
                    var waypoint = waypoints.Value[i];
                    if (waypoint == null) {
                        continue;
                    }

                    var localDistance = Vector3.SqrMagnitude(transform.position - waypoint.transform.position);
                    if (localDistance < distance) {
                        distance = localDistance;
                        m_WaypointIndex = i;
                    }
                }
            }

            base.OnStart();
        }

        public override TaskStatus OnUpdate()
        {
            if (waypoints == null || waypoints.Value == null || waypoints.Value.Count == 0) {
                return TaskStatus.Failure;
            }

            if (m_PauseTime > 0f) {
                m_PauseTime -= Time.deltaTime;
                return TaskStatus.Running;
            }

            if (navMeshAgent == null || !navMeshAgent.enabled) {
                return TaskStatus.Failure;
            }

            if (m_PauseTime <= 0f && !navMeshAgent.pathPending &&
                navMeshAgent.remainingDistance <= arriveDistance.Value) {
                if (randomPatrol.Value) {
                    m_WaypointIndex = Random.Range(0, waypoints.Value.Count);
                } else {
                    m_WaypointIndex = (m_WaypointIndex + 1) % waypoints.Value.Count;
                }

                m_PauseTime = waypointPauseDuration.Value;
                SetDestination(GetDestination());
            }

            return TaskStatus.Running;
        }

        public override void OnReset()
        {
            base.OnReset();
            randomPatrol = false;
            waypointPauseDuration = 1f;
            waypoints = null;
        }
    }
}
