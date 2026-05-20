using UnityEngine;

[DisallowMultipleComponent]
public sealed class ColliderActivatorComponent : MonoBehaviour
{
    [SerializeField] private ColliderComponent _colliderComponent;

    public void SetActive(bool active) => _colliderComponent.Collider.enabled = active;
}