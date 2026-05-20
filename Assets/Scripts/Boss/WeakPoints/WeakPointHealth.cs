using System;
using UnityEngine;

/// <summary>
/// Weak point health data and damage application. Notifies observers via <see cref="OnHealthChanged"/>.
/// </summary>
[DisallowMultipleComponent]
public class WeakPointHealth : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 30f;
    [SerializeField] private float _bossDamageOnDestroy = 100f;

    private float _currentHealth;

    public float MaxHealth => _maxHealth;
    public float CurrentHealth => _currentHealth;
    public float BossDamageOnDestroy => _bossDamageOnDestroy;
    public bool IsDestroyed => _currentHealth <= 0f;

    public event Action<WeakPointHealthChange> OnHealthChanged;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public bool ApplyDamage(float amount)
    {
        if (IsDestroyed || !isActiveAndEnabled || !gameObject.activeInHierarchy || amount <= 0f) {
            return false;
        }

        var previous = _currentHealth;
        _currentHealth = Mathf.Max(0f, _currentHealth - amount);
        RaiseHealthChanged(previous, wasReset: false);
        return true;
    }

    public void ResetHealth()
    {
        var previous = _currentHealth;
        _currentHealth = _maxHealth;
        RaiseHealthChanged(previous, wasReset: true);
    }

    private void RaiseHealthChanged(float previousHealth, bool wasReset)
    {
        OnHealthChanged?.Invoke(new WeakPointHealthChange(previousHealth, _currentHealth, _maxHealth, wasReset));
    }
}
