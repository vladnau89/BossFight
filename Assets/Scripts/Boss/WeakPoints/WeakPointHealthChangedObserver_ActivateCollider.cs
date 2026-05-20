using UnityEngine;

/// <summary>
/// Observes <see cref="WeakPointHealth"/> and enables or disables the weak point collider.
/// </summary>
[DisallowMultipleComponent]
public class WeakPointHealthChangedObserver_ActivateCollider : MonoBehaviour
{
    [SerializeField] private WeakPointHealth _health;
    [SerializeField] private TriggerComponent _trigger;

    private Collider Collider => _trigger.Collider;
    
    private void OnEnable()
    {
        _health.OnHealthChanged += HandleHealthChanged;
    }

    private void OnDisable()
    {
        _health.OnHealthChanged -= HandleHealthChanged;
    }

    private void HandleHealthChanged(WeakPointHealthChange change)
    {
        if (change.WasReset) 
        {
            SetColliderEnabled(true);
            return;
        }

        if (change.WasDestroyed) 
        {
            SetColliderEnabled(false);
        }
    }

    private void SetColliderEnabled(bool enable) => Collider.enabled = enable;
}
