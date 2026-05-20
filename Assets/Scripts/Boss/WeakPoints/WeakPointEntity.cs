using UnityEngine;

/// <summary>
/// Facade for boss combat: collider lookup, damage routing, activation.
/// </summary>
[DisallowMultipleComponent]
public class WeakPointEntity : MonoBehaviour
{
    [SerializeField] private WeakPointHealth _health;
    [SerializeField] private TriggerComponent _trigger;

    public Collider Collider => _trigger.Collider;
    public float BossDamageOnDestroy => _health.BossDamageOnDestroy;
    public bool IsDestroyed => _health.IsDestroyed;
    
    public bool TakeDamage(float amount) => _health.ApplyDamage(amount);

    public void ResetHealth() => _health.ResetHealth();

    public void SetActive(bool active) => gameObject.SetActive(active);
}
