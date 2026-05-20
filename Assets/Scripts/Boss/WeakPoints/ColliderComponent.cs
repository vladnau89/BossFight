using UnityEngine;

/// <summary>
/// Holds the weak point hit collider reference for combat and surface lookup.
/// </summary>
[DisallowMultipleComponent]
public sealed class ColliderComponent : MonoBehaviour
{
    [SerializeField] private Collider _collider;

    public Collider Collider => _collider;
}