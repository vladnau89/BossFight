/// <summary>
/// Snapshot passed to weak point health observers.
/// </summary>
public readonly struct WeakPointHealthChange
{
    public float PreviousHealth { get; }
    public float CurrentHealth { get; }
    public float MaxHealth { get; }

    public bool WasReset { get; }
    public bool WasHit { get; }
    public bool WasDestroyed { get; }

    public WeakPointHealthChange(float previousHealth, float currentHealth, float maxHealth, bool wasReset)
    {
        PreviousHealth = previousHealth;
        CurrentHealth = currentHealth;
        MaxHealth = maxHealth;
        WasReset = wasReset;
        WasHit = !wasReset && currentHealth > 0f && currentHealth < previousHealth;
        WasDestroyed = !wasReset && currentHealth <= 0f && previousHealth > 0f;
    }
}
