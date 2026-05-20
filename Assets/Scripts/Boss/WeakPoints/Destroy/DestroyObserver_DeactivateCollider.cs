using UnityEngine;

[DisallowMultipleComponent]
public class DestroyObserver_DeactivateCollider : MonoBehaviour
{
    [SerializeField] private DestroyComponent _destroyComponent;
    [SerializeField] private ColliderActivatorComponent _colliderActivator;

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
        _colliderActivator.SetActive(false);
    }
}