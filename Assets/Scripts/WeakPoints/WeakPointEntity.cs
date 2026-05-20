using UnityEngine;

/// <summary>
/// Facade for boss combat: collider lookup, damage routing, activation.
/// </summary>
[DisallowMultipleComponent]
public class WeakPointEntity : MonoBehaviour
{
    [SerializeField] private HealthComponent _health;
    [SerializeField] private DamageValueComponent _damageValue;
    [SerializeField] private ColliderComponent _trigger;
    [SerializeField] private GameObjectActivator _activator;
    [SerializeField] private DestroyComponent _destroy;

    public Collider Collider => _trigger.Collider;
    public float BossDamageOnDestroy => _damageValue.DamageValue;
    public bool IsDestroyed => _destroy.IsDestroyed;
    
    public bool TakeDamage(float amount) => _health.ChangeHealth(-amount);

    public void ResetHealth() => _health.ResetHealth();

    public void SetActive(bool active) => _activator.SetActive(active);
}
