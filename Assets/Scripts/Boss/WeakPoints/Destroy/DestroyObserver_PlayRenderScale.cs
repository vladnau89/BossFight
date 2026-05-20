using UnityEngine;

[DisallowMultipleComponent]
public sealed class DestroyObserver_PlayRenderScale : MonoBehaviour
{
    [SerializeField] private DestroyComponent _destroyComponent;
    [SerializeField] private DestroyVisualComponent _destroyVisualComponent;
    [SerializeField] private RenderScale _renderScale;

    private void OnEnable()
    {
        _destroyComponent.EventDestroyed += OnEventDestroyed;
    }

    private void OnDisable()
    {
        _destroyComponent.EventDestroyed -= OnEventDestroyed;
    }

    private void OnEventDestroyed() => _renderScale.Play(_destroyVisualComponent.DestroyFlashDuration);
}