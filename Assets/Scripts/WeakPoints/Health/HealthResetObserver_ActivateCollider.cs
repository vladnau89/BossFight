using UnityEngine;


[DisallowMultipleComponent]
public class HealthResetObserver_ActivateCollider : MonoBehaviour
{
    [SerializeField] private HealthComponent _health;
    [SerializeField] private ColliderActivatorComponent _colliderActivator;
    
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
        
        _colliderActivator.SetActive(true);
    }

}