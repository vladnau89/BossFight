using UnityEngine;

[DisallowMultipleComponent]
public class DamageValueComponent : MonoBehaviour
{
    [SerializeField] private float Damage = 100f;

    public float DamageValue => Damage;

    public void ApplySettings(float damage) => Damage = damage;
}

