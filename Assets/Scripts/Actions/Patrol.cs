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

        private int _waypointIndex;
        private float _pauseTime = -1f;

        protected override Vector3 GetDestination()
        {
            if (waypoints == null || waypoints.Value == null || waypoints.Value.Count == 0) {
                return transform.position;
            }

            var waypoint = waypoints.Value[_waypointIndex];
            return waypoint != null ? waypoint.transform.position : transform.position;
        }

        public override void OnStart()
        {
            _pauseTime = -1f;
            _waypointIndex = 0;

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
                        _waypointIndex = i;
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

            if (_pauseTime > 0f) {
                _pauseTime -= Time.deltaTime;
                return TaskStatus.Running;
            }

            if (navMeshAgent == null || !navMeshAgent.enabled) {
                return TaskStatus.Failure;
            }

            if (_pauseTime <= 0f && !navMeshAgent.pathPending &&
                navMeshAgent.remainingDistance <= arriveDistance.Value) {
                if (randomPatrol.Value) {
                    _waypointIndex = Random.Range(0, waypoints.Value.Count);
                } else {
                    _waypointIndex = (_waypointIndex + 1) % waypoints.Value.Count;
                }

                _pauseTime = waypointPauseDuration.Value;
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
