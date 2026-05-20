using UnityEngine;

[DisallowMultipleComponent]
public sealed class HealthChangedObserver_ToDestroy : MonoBehaviour
{
    [SerializeField] private HealthComponent _health;
    [SerializeField] private DestroyComponent _destroy;


    private void OnEnable()
    {
        _health.EventHealthChanged += OnHealthChanged;
    }

    private void OnHealthChanged(HealthChangeEventData data)
    {
        if (data.WasDestroyed && !_destroy.IsDestroyed)
        {
            _destroy.ToDestroy();
            return;
        }

        if (data.WasReset)
        {
            _destroy.ResetDestroy();
        }
    }
}