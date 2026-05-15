using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Determines if a target is within the distance specified.")]
    [TaskCategory("Movement")]
    public class WithinDistance : Conditional
    {
        [Tooltip("Should the 2D version be used?")]
        public bool usePhysics2D;

        [Tooltip("The object to check.")]
        public SharedGameObject targetObject;

        [Tooltip("If the object is null then find objects by tag.")]
        public SharedString targetTag;

        [Tooltip("The LayerMask of the objects that are searched for.")]
        public LayerMask objectLayerMask = -1;

        [Tooltip("The distance that the object needs to be within.")]
        public SharedFloat magnitude = 5f;

        [Tooltip("If true, the object must be within line of sight.")]
        public SharedBool lineOfSight;

        [Tooltip("The LayerMask to ignore.")]
        public LayerMask ignoreLayerMask;

        [Tooltip("The offset relative to the pivot position.")]
        public SharedVector3 offset;

        [Tooltip("The target offset relative to the target pivot position.")]
        public SharedVector3 targetOffset;

        [Tooltip("Draw a debug ray in the scene view.")]
        public SharedBool drawDebugRay;

        [Tooltip("The object that is within distance.")]
        public SharedGameObject returnedObject;

        public override TaskStatus OnUpdate()
        {
            if (usePhysics2D) {
                Debug.LogWarning("WithinDistance: usePhysics2D is not supported in this replacement implementation.");
            }

            var target = targetObject != null ? targetObject.Value : null;
            if (target == null && !string.IsNullOrEmpty(targetTag.Value)) {
                target = MovementUtility.FindTargetByTag(targetTag.Value, objectLayerMask);
            }

            if (target == null) {
                returnedObject.Value = null;
                return TaskStatus.Failure;
            }

            var withinDistance = MovementUtility.WithinDistance(transform, offset.Value, magnitude.Value, target,
                targetOffset.Value, lineOfSight.Value, ignoreLayerMask, false);

            if (withinDistance) {
                returnedObject.Value = target;
                return TaskStatus.Success;
            }

            returnedObject.Value = null;
            return TaskStatus.Failure;
        }

        public override void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (magnitude == null) {
                return;
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + offset.Value, magnitude.Value);
#endif
        }

        public override void OnReset()
        {
            usePhysics2D = false;
            targetObject = null;
            targetTag = string.Empty;
            objectLayerMask = -1;
            magnitude = 5f;
            lineOfSight = false;
            ignoreLayerMask = 0;
            offset = Vector3.zero;
            targetOffset = Vector3.zero;
            drawDebugRay = false;
            returnedObject = null;
        }
    }
}
