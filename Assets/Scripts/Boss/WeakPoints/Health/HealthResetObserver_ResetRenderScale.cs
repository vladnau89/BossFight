using UnityEngine;


[DisallowMultipleComponent]
public sealed class HealthResetObserver_ResetRenderScale : MonoBehaviour
{
    [SerializeField] private HealthComponent _health;
    [SerializeField] private RenderScale _renderScale;

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
        _renderScale.ResetScale();
    }
}