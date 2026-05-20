using UnityEngine;

[DisallowMultipleComponent]
public sealed class HealthResetObserver_ResetRenderPulse : MonoBehaviour
{
    [SerializeField] private HealthComponent _health;
    [SerializeField] private RenderPulse _renderPulse;

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
        _renderPulse.ResetPulse();
    }
}