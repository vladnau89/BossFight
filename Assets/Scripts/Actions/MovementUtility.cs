using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    /// <summary>
    /// Helper methods for Movement tasks (compatible with Behavior Designer Movement Pack serialization).
    /// </summary>
    public static class MovementUtility
    {
        private static readonly Collider[] s_OverlapResults = new Collider[200];

        public static GameObject WithinSight(Transform transform, Vector3 offset, float fieldOfViewAngle, float viewDistance,
            LayerMask objectLayerMask, int maxCollisionCount, LayerMask ignoreLayerMask, Vector3 targetOffset,
            float angleOffset2D, bool disableAgentColliderLayer, ref int overlapCount)
        {
            overlapCount = Physics.OverlapSphereNonAlloc(transform.position, viewDistance, s_OverlapResults, objectLayerMask);
            overlapCount = Mathf.Min(overlapCount, maxCollisionCount > 0 ? maxCollisionCount : overlapCount);

            for (var i = 0; i < overlapCount; ++i) {
                var collider = s_OverlapResults[i];
                if (collider == null || collider.transform == transform) {
                    continue;
                }

                if (IsWithinSight(transform, offset, fieldOfViewAngle, viewDistance, collider.transform, targetOffset,
                        ignoreLayerMask, disableAgentColliderLayer)) {
                    return collider.gameObject;
                }
            }

            return null;
        }

        public static bool IsWithinSight(Transform transform, Vector3 offset, float fieldOfViewAngle, float viewDistance,
            Transform targetTransform, Vector3 targetOffset, LayerMask ignoreLayerMask, bool disableAgentColliderLayer)
        {
            if (targetTransform == null) {
                return false;
            }

            var origin = transform.position + offset;
            var targetPosition = targetTransform.position + targetOffset;
            var direction = targetPosition - origin;
            var distance = direction.magnitude;

            if (distance > viewDistance || distance <= Mathf.Epsilon) {
                return false;
            }

            direction /= distance;

            if (Vector3.Angle(transform.forward, direction) > fieldOfViewAngle * 0.5f) {
                return false;
            }

            return HasLineOfSight(origin, targetPosition, ignoreLayerMask, disableAgentColliderLayer ? transform : null);
        }

        public static bool IsWithinSight(Transform transform, Vector3 offset, float fieldOfViewAngle, float viewDistance,
            GameObject target, Vector3 targetOffset, bool useTargetBone, HumanBodyBones targetBone, LayerMask ignoreLayerMask,
            bool disableAgentColliderLayer)
        {
            if (target == null) {
                return false;
            }

            var targetPosition = target.transform.position + targetOffset;
            if (useTargetBone) {
                var animator = target.GetComponent<Animator>();
                if (animator != null) {
                    var bone = animator.GetBoneTransform(targetBone);
                    if (bone != null) {
                        targetPosition = bone.position;
                    }
                }
            }

            var origin = transform.position + offset;
            var direction = targetPosition - origin;
            var distance = direction.magnitude;

            if (distance > viewDistance || distance <= Mathf.Epsilon) {
                return false;
            }

            direction /= distance;

            if (Vector3.Angle(transform.forward, direction) > fieldOfViewAngle * 0.5f) {
                return false;
            }

            return HasLineOfSight(origin, targetPosition, ignoreLayerMask, disableAgentColliderLayer ? transform : null);
        }

        public static bool HasLineOfSight(Vector3 origin, Vector3 targetPosition, LayerMask ignoreLayerMask, Transform agentTransform)
        {
            var direction = targetPosition - origin;
            var distance = direction.magnitude;
            if (distance <= Mathf.Epsilon) {
                return true;
            }

            if (Physics.Raycast(origin, direction / distance, out var hit, distance, ~ignoreLayerMask.value, QueryTriggerInteraction.Ignore)) {
                if (agentTransform != null && hit.transform.IsChildOf(agentTransform)) {
                    return HasLineOfSight(hit.point + direction.normalized * 0.01f, targetPosition, ignoreLayerMask, null);
                }

                return Vector3.SqrMagnitude(hit.point - targetPosition) < 0.25f;
            }

            return true;
        }

        public static bool WithinDistance(Transform transform, Vector3 offset, float magnitude, GameObject targetObject,
            Vector3 targetOffset, bool lineOfSight, LayerMask ignoreLayerMask, bool disableAgentColliderLayer)
        {
            if (targetObject == null) {
                return false;
            }

            var direction = targetObject.transform.position + targetOffset - (transform.position + offset);
            if (direction.sqrMagnitude > magnitude * magnitude) {
                return false;
            }

            if (!lineOfSight) {
                return true;
            }

            return HasLineOfSight(transform.position + offset, targetObject.transform.position + targetOffset,
                ignoreLayerMask, disableAgentColliderLayer ? transform : null);
        }

        public static GameObject FindTargetByTag(string tag, LayerMask objectLayerMask)
        {
            if (string.IsNullOrEmpty(tag)) {
                return null;
            }

            var objects = GameObject.FindGameObjectsWithTag(tag);
            for (var i = 0; i < objects.Length; ++i) {
                if (((1 << objects[i].layer) & objectLayerMask) != 0) {
                    return objects[i];
                }
            }

            return null;
        }

        public static void DrawLineOfSight(Transform transform, Vector3 offset, float fieldOfViewAngle, float viewDistance)
        {
#if UNITY_EDITOR
            var origin = transform.position + offset;
            var left = Quaternion.AngleAxis(-fieldOfViewAngle * 0.5f, transform.up) * transform.forward * viewDistance;
            var right = Quaternion.AngleAxis(fieldOfViewAngle * 0.5f, transform.up) * transform.forward * viewDistance;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(origin, origin + left);
            Gizmos.DrawLine(origin, origin + right);
#endif
        }
    }
}
