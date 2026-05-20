using UnityEngine;

[DisallowMultipleComponent]
public sealed class HealthHitObserver_PlayRenderPulse : MonoBehaviour
{
    [SerializeField] private HealthComponent _health;
    [SerializeField] private DestroyVisualComponent _destroyVisualComponent;
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
        if (!changeEventData.WasHit)
        {
            return;
        }
        _renderPulse.Play(_destroyVisualComponent.HitFlashDuration);
    }
}