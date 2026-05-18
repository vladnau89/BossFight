using UnityEngine;

/// <summary>
/// Stores the homing target set by Behavior Designer before a projectile is fired.
/// </summary>
public class HomingTargetProvider : MonoBehaviour
{
    [field: SerializeField] public Transform HomingTarget { get; set; }

    public void SetHomingTarget(Transform target)
    {
        HomingTarget = target;
    }

    public void ClearHomingTarget()
    {
        HomingTarget = null;
    }
}
