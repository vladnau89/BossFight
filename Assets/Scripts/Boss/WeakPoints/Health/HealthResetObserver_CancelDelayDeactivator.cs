using UnityEngine;

[DisallowMultipleComponent]
public sealed class HealthResetObserver_CancelDelayDeactivator : MonoBehaviour
{
    [SerializeField] private HealthComponent _health;
    [SerializeField] private DelayGameObjectActivator _activator;

    private void Awake()
    {
        _health.EventHealthChanged += HandleHealthChanged;
    }

    private void OnDestroy()
    {
        _health.EventHealthChanged -= HandleHealthChanged;
    }

    private void HandleHealthChanged(HealthChangeEventData changeEventData)
    {
        if (!changeEventData.WasReset)
        {
            return;
        }

        _activator.Cancel();
    }
}