using UnityEngine;

[DisallowMultipleComponent]
public sealed class DestroyObserver_PlayRenderPulse : MonoBehaviour
{
    [SerializeField] private DestroyComponent _destroyComponent;
    [SerializeField] private DestroyVisualComponent _destroyVisualComponent;
    [SerializeField] private RenderPulse _renderPulse;

    private void OnEnable()
    {
        _destroyComponent.EventDestroyed += OnEventDestroyed;
    }

    private void OnDisable()
    {
        _destroyComponent.EventDestroyed -= OnEventDestroyed;
    }

    private void OnEventDestroyed() => _renderPulse.Play(_destroyVisualComponent.DestroyFlashDuration);
}