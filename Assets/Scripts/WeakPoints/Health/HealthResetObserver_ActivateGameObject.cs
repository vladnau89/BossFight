using UnityEngine;

[DisallowMultipleComponent]
public sealed class HealthResetObserver_ActivateGameObject : MonoBehaviour
{
    [SerializeField] private HealthComponent _health;
    [SerializeField] private GameObjectActivator _activator;

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

        _activator.SetActive(true);
    }
}