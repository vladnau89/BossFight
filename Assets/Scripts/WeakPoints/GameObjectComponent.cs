using UnityEngine;

[DisallowMultipleComponent]
public sealed class GameObjectComponent : MonoBehaviour
{
    [SerializeField] private GameObject _gameObject;

    public GameObject GameObject => _gameObject;
}