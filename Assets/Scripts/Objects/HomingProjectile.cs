using Opsive.UltimateCharacterController.Objects;
using Opsive.UltimateCharacterController.SurfaceSystem;
using UnityEngine;

/// <summary>
/// UCC projectile that steers toward a target within a limited cone, then flies on and self-destructs if the target is missed.
/// </summary>
public class HomingProjectile : Projectile
{
    [Tooltip("Maximum turn speed in degrees per second.")]
    [SerializeField] private float m_TurnRate = 90f;

    [Tooltip("Stop homing if the target is farther than this distance.")]
    [SerializeField] private float m_MaxHomingDistance = 150f;

    [Tooltip("Homing stops when the target leaves this cone around the initial launch direction.")]
    [SerializeField] private float m_MaxHomingConeAngle = 60f;

    [Tooltip("The flight direction can never turn more than this angle away from the launch direction.")]
    [SerializeField] private float m_MaxDeflectionAngle = 45f;

    [Tooltip("Stop homing once the target is behind the rocket's current flight direction.")]
    [SerializeField] private bool m_StopHomingWhenTargetBehind = true;

    [Tooltip("Rotate the mesh to face the movement direction while homing.")]
    [SerializeField] private bool m_RotateTowardsVelocity = true;

    [SerializeField] private Transform m_HomingTarget;

    private Vector3 m_LaunchDirection;
    private bool m_HomingEnabled;

    /// <summary>
    /// Sets the transform that this projectile should home in on.
    /// </summary>
    /// <param name="target">The target transform.</param>
    public void SetHomingTarget(Transform target)
    {
        m_HomingTarget = target;
    }

    /// <summary>
    /// Initializes the projectile and reads the homing target from the shooter.
    /// </summary>
    public override void Initialize(Vector3 velocity, Vector3 torque, float damageAmount, float impactForce, int impactForceFrames,
        LayerMask impactLayers, string impactStateName, float impactStateDisableTimer, SurfaceImpact surfaceImpact, GameObject originator)
    {
        base.Initialize(velocity, torque, damageAmount, impactForce, impactForceFrames, impactLayers, impactStateName,
            impactStateDisableTimer, surfaceImpact, originator);

        m_LaunchDirection = velocity.sqrMagnitude > 0.0001f ? velocity.normalized : m_Transform.forward;
        AcquireTarget(originator);
        m_HomingEnabled = m_HomingTarget != null;
    }

    /// <summary>
    /// Steers the projectile before UCC updates its trajectory.
    /// </summary>
    protected override void FixedUpdate()
    {
        ApplyHoming();
        base.FixedUpdate();
    }

    private void AcquireTarget(GameObject originator)
    {
        m_HomingTarget = null;
        if (originator == null) {
            return;
        }

        var provider = originator.GetComponent<HomingTargetProvider>();
        if (provider != null && provider.HomingTarget != null) {
            SetHomingTarget(provider.HomingTarget);
        }
    }

    private void StopHoming()
    {
        m_HomingEnabled = false;
        m_HomingTarget = null;
    }

    private void ApplyHoming()
    {
        if (!m_HomingEnabled || m_HomingTarget == null) {
            return;
        }

        var toTarget = m_HomingTarget.position - m_Transform.position;
        var sqrDistance = toTarget.sqrMagnitude;
        if (sqrDistance > m_MaxHomingDistance * m_MaxHomingDistance) {
            StopHoming();
            return;
        }

        if (sqrDistance < 0.0001f) {
            return;
        }

        var speed = m_Velocity.magnitude;
        if (speed < 0.01f) {
            return;
        }

        var desiredDirection = toTarget.normalized;
        var currentDirection = m_Velocity / speed;

        if (Vector3.Angle(m_LaunchDirection, desiredDirection) > m_MaxHomingConeAngle) {
            StopHoming();
            return;
        }

        if (m_StopHomingWhenTargetBehind && Vector3.Dot(currentDirection, desiredDirection) <= 0f) {
            StopHoming();
            return;
        }

        var maxRadians = m_TurnRate * Mathf.Deg2Rad * Time.fixedDeltaTime;
        var newDirection = Vector3.RotateTowards(currentDirection, desiredDirection, maxRadians, 0f);

        var maxDeflectionRadians = m_MaxDeflectionAngle * Mathf.Deg2Rad;
        if (Vector3.Angle(m_LaunchDirection, newDirection) > m_MaxDeflectionAngle) {
            newDirection = Vector3.RotateTowards(m_LaunchDirection, newDirection, maxDeflectionRadians, 0f);
        }

        m_Velocity = newDirection * speed;

        if (m_RotateTowardsVelocity) {
            m_Transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }
}
