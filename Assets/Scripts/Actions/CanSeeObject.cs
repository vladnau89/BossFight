using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Returns success if an object on objectLayerMask is within viewDistance.")]
    [TaskCategory("Movement")]
    public class CanSeeObject : Conditional
    {
        [Tooltip("The object to search for. If null the layer mask is used.")]
        public SharedGameObject targetObject;

        [Tooltip("An array of objects to search for.")]
        public SharedGameObjectList targetObjects;

        [Tooltip("If the object list is empty then find objects by tag.")]
        public SharedString targetTag;

        [Tooltip("The LayerMask of the objects that are searched for.")]
        public LayerMask objectLayerMask = -1;

        [Tooltip("The maximum number of colliders to check.")]
        public int maxCollisionCount = 200;

        [Tooltip("The distance that the agent can see.")]
        public SharedFloat viewDistance = 1000f;

        [Tooltip("The offset relative to the pivot position.")]
        public SharedVector3 offset;

        [Tooltip("The object that is within sight.")]
        public SharedGameObject returnedObject;
        
        public SharedBool drawDebugRay;

        private static readonly Collider[] s_OverlapResults = new Collider[200];

        public override TaskStatus OnUpdate()
        {
            var origin = transform.position + offset.Value;
            var viewDistanceSqr = viewDistance.Value * viewDistance.Value;
            GameObject seenObject = null;

            if (targetObject != null && targetObject.Value != null) {
                seenObject = IsWithinDistance(origin, viewDistanceSqr, targetObject.Value) ? targetObject.Value : null;
            } else if (targetObjects != null && targetObjects.Value != null && targetObjects.Value.Count > 0) {
                for (var i = 0; i < targetObjects.Value.Count; ++i) {
                    var obj = targetObjects.Value[i];
                    if (obj != null && IsWithinDistance(origin, viewDistanceSqr, obj)) {
                        seenObject = obj;
                        break;
                    }
                }
            } else if (!string.IsNullOrEmpty(targetTag.Value)) {
                var objects = GameObject.FindGameObjectsWithTag(targetTag.Value);
                for (var i = 0; i < objects.Length; ++i) {
                    if (IsWithinDistance(origin, viewDistanceSqr, objects[i])) {
                        seenObject = objects[i];
                        break;
                    }
                }
            } else {
                var count = Physics.OverlapSphereNonAlloc(origin, viewDistance.Value, s_OverlapResults, objectLayerMask);
                var limit = maxCollisionCount > 0 ? Mathf.Min(count, maxCollisionCount) : count;
                for (var i = 0; i < limit; ++i) {
                    var collider = s_OverlapResults[i];
                    if (collider == null || collider.transform == transform) {
                        continue;
                    }

                    seenObject = collider.gameObject;
                    break;
                }
            }

            returnedObject.Value = seenObject;
            return seenObject != null ? TaskStatus.Success : TaskStatus.Failure;
        }

        private static bool IsWithinDistance(Vector3 origin, float viewDistanceSqr, GameObject target)
        {
            return (target.transform.position - origin).sqrMagnitude <= viewDistanceSqr;
        }

        public override void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (drawDebugRay != null && drawDebugRay.Value) {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position + offset.Value, viewDistance.Value);
            }
#endif
        }

        public override void OnReset()
        {
            targetObject = null;
            targetObjects = null;
            targetTag = string.Empty;
            objectLayerMask = -1;
            maxCollisionCount = 200;
            viewDistance = 1000f;
            offset = Vector3.zero;
            returnedObject = null;
        }
    }
}
