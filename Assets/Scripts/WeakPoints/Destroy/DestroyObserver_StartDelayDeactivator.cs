using UnityEngine;

[DisallowMultipleComponent]
public sealed class DestroyObserver_StartDelayDeactivator : MonoBehaviour
{
    [SerializeField] private DestroyComponent _destroy;
    [SerializeField] private DelayGameObjectActivator _delayGameObjectActivator;

    private void OnEnable()
    {
        _destroy.EventDestroyed += OnEventDestroyed;
    }

    private void OnDisable()
    {
        _destroy.EventDestroyed -= OnEventDestroyed;
    }

    private void OnEventDestroyed()
    {
        _delayGameObjectActivator.Process();
    }
}