using UnityEngine;

[DisallowMultipleComponent]
public sealed class GameObjectActivator : MonoBehaviour
{
    [SerializeField] private GameObjectComponent _gameObjectComponent;

    public void SetActive(bool active) => _gameObjectComponent.GameObject.SetActive(active);
}