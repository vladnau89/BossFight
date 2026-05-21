using System;
using UnityEngine;


[DisallowMultipleComponent]
public class HealthComponent : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 30f;
    [SerializeField] private float _currentHealth;
    
    public event Action<HealthChangeEventData> EventHealthChanged;

    private void Awake()
    {
        ResetHealth();
    }

    public bool ChangeHealth(float delta)
    {
        var previous = _currentHealth;
        _currentHealth = Mathf.Clamp(_currentHealth + delta, 0f, _maxHealth);
        RaiseHealthChanged(previous, wasReset: false);
        return true;
    }

    public void ResetHealth()
    {
        var previous = _currentHealth;
        _currentHealth = _maxHealth;
        RaiseHealthChanged(previous, wasReset: true);
    }

    public void ApplySettings(float maxHealth)
    {
        _maxHealth = maxHealth;
        ResetHealth();
    }

    private void RaiseHealthChanged(float previousHealth, bool wasReset)
    {
        EventHealthChanged?.Invoke(new HealthChangeEventData(previousHealth, _currentHealth, _maxHealth, wasReset));
    }
}
