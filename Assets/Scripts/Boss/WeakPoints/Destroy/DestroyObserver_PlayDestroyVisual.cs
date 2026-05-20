using UnityEngine;

[DisallowMultipleComponent]
public class DestroyObserver_PlayDestroyVisual : MonoBehaviour
{
    [SerializeField] private DestroyComponent _destroyComponent;
    [SerializeField] private WeakPointVisual _visual;

    private void OnEnable()
    {
        _destroyComponent.EventDestroyed += OnEventDestroyed;
    }

    private void OnDisable()
    {
        _destroyComponent.EventDestroyed -= OnEventDestroyed;
    }

    private void OnEventDestroyed()
    {
        _visual.PlayDestroyed();
    }
}