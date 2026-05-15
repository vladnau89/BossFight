using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Determines if any objects are within hearing range of the agent.")]
    [TaskCategory("Movement")]
    public class CanHearObject : Conditional
    {
        [Tooltip("Should the 2D version be used?")]
        public bool usePhysics2D;

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

        [Tooltip("The hearing radius.")]
        public SharedFloat hearingRadius = 50f;

        [Tooltip("The audibility threshold (0-1).")]
        public SharedFloat audibilityThreshold = 0.15f;

        [Tooltip("The offset relative to the pivot position.")]
        public SharedVector3 offset;

        [Tooltip("The object that was heard.")]
        public SharedGameObject returnedObject;

        private static readonly Collider[] s_OverlapResults = new Collider[200];

        public override TaskStatus OnUpdate()
        {
            if (usePhysics2D) {
                Debug.LogWarning("CanHearObject: usePhysics2D is not supported in this replacement implementation.");
            }

            GameObject heardObject = null;
            var origin = transform.position + offset.Value;
            var hearingRadiusSqr = hearingRadius.Value * hearingRadius.Value;

            if (targetObject != null && targetObject.Value != null) {
                if (IsAudible(origin, targetObject.Value, hearingRadiusSqr)) {
                    heardObject = targetObject.Value;
                }
            } else if (targetObjects != null && targetObjects.Value != null && targetObjects.Value.Count > 0) {
                for (var i = 0; i < targetObjects.Value.Count; ++i) {
                    var obj = targetObjects.Value[i];
                    if (obj != null && IsAudible(origin, obj, hearingRadiusSqr)) {
                        heardObject = obj;
                        break;
                    }
                }
            } else if (!string.IsNullOrEmpty(targetTag.Value)) {
                var objects = GameObject.FindGameObjectsWithTag(targetTag.Value);
                for (var i = 0; i < objects.Length; ++i) {
                    if (IsAudible(origin, objects[i], hearingRadiusSqr)) {
                        heardObject = objects[i];
                        break;
                    }
                }
            } else {
                var count = Physics.OverlapSphereNonAlloc(origin, hearingRadius.Value, s_OverlapResults, objectLayerMask);
                count = Mathf.Min(count, maxCollisionCount > 0 ? maxCollisionCount : count);
                for (var i = 0; i < count; ++i) {
                    var collider = s_OverlapResults[i];
                    if (collider == null || collider.transform == transform) {
                        continue;
                    }

                    if (IsAudible(origin, collider.gameObject, hearingRadiusSqr)) {
                        heardObject = collider.gameObject;
                        break;
                    }
                }
            }

            returnedObject.Value = heardObject;
            return heardObject != null ? TaskStatus.Success : TaskStatus.Failure;
        }

        private bool IsAudible(Vector3 origin, GameObject target, float hearingRadiusSqr)
        {
            if (target == null) {
                return false;
            }

            if ((target.transform.position - origin).sqrMagnitude > hearingRadiusSqr) {
                return false;
            }

            var audioSource = target.GetComponent<AudioSource>();
            if (audioSource != null && audioSource.isPlaying && audioSource.volume < audibilityThreshold.Value) {
                return false;
            }

            return true;
        }

        public override void OnReset()
        {
            usePhysics2D = false;
            targetObject = null;
            targetObjects = null;
            targetTag = string.Empty;
            objectLayerMask = -1;
            maxCollisionCount = 200;
            hearingRadius = 50f;
            audibilityThreshold = 0.15f;
            offset = Vector3.zero;
            returnedObject = null;
        }
    }
}
