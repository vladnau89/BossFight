using Opsive.UltimateCharacterController.Objects;
using Opsive.UltimateCharacterController.SurfaceSystem;
using UnityEngine;

/// <summary>
/// UCC projectile that steers toward a target within a limited cone, then flies on and self-destructs if the target is missed.
/// </summary>
public class HomingProjectile : Projectile
{
    [Tooltip("Maximum turn speed in degrees per second.")]
    [SerializeField] private float _turnRate = 90f;

    [Tooltip("Stop homing if the target is farther than this distance.")]
    [SerializeField] private float _maxHomingDistance = 150f;

    [Tooltip("Homing stops when the target leaves this cone around the initial launch direction.")]
    [SerializeField] private float _maxHomingConeAngle = 60f;

    [Tooltip("The flight direction can never turn more than this angle away from the launch direction.")]
    [SerializeField] private float _maxDeflectionAngle = 45f;

    [Tooltip("Stop homing once the target is behind the rocket's current flight direction.")]
    [SerializeField] private bool _stopHomingWhenTargetBehind = true;

    [Tooltip("Rotate the mesh to face the movement direction while homing.")]
    [SerializeField] private bool _rotateTowardsVelocity = true;

    [SerializeField] private Transform _homingTarget;

    private Vector3 _launchDirection;
    private bool _homingEnabled;

    /// <summary>
    /// Sets the transform that this projectile should home in on.
    /// </summary>
    /// <param name="target">The target transform.</param>
    public void SetHomingTarget(Transform target)
    {
        _homingTarget = target;
    }

    /// <summary>
    /// Initializes the projectile and reads the homing target from the shooter.
    /// </summary>
    public override void Initialize(Vector3 velocity, Vector3 torque, float damageAmount, float impactForce, int impactForceFrames,
        LayerMask impactLayers, string impactStateName, float impactStateDisableTimer, SurfaceImpact surfaceImpact, GameObject originator)
    {
        base.Initialize(velocity, torque, damageAmount, impactForce, impactForceFrames, impactLayers, impactStateName,
            impactStateDisableTimer, surfaceImpact, originator);

        _launchDirection = velocity.sqrMagnitude > 0.0001f ? velocity.normalized : m_Transform.forward;
        AcquireTarget(originator);
        _homingEnabled = _homingTarget != null;
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
        _homingTarget = null;
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
        _homingEnabled = false;
        _homingTarget = null;
    }

    private void ApplyHoming()
    {
        if (!_homingEnabled || _homingTarget == null) {
            return;
        }

        var toTarget = _homingTarget.position - m_Transform.position;
        var sqrDistance = toTarget.sqrMagnitude;
        if (sqrDistance > _maxHomingDistance * _maxHomingDistance) {
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

        if (Vector3.Angle(_launchDirection, desiredDirection) > _maxHomingConeAngle) {
            StopHoming();
            return;
        }

        if (_stopHomingWhenTargetBehind && Vector3.Dot(currentDirection, desiredDirection) <= 0f) {
            StopHoming();
            return;
        }

        var maxRadians = _turnRate * Mathf.Deg2Rad * Time.fixedDeltaTime;
        var newDirection = Vector3.RotateTowards(currentDirection, desiredDirection, maxRadians, 0f);

        var maxDeflectionRadians = _maxDeflectionAngle * Mathf.Deg2Rad;
        if (Vector3.Angle(_launchDirection, newDirection) > _maxDeflectionAngle) {
            newDirection = Vector3.RotateTowards(_launchDirection, newDirection, maxDeflectionRadians, 0f);
        }

        m_Velocity = newDirection * speed;

        if (_rotateTowardsVelocity) {
            m_Transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }
}
