using System;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class DestroyComponent : MonoBehaviour
{
    [SerializeField] private bool _isDestroyed = false;

    public bool IsDestroyed => _isDestroyed;

    public event Action EventDestroyed;

    public void ToDestroy()
    {
        _isDestroyed = true;
        EventDestroyed?.Invoke();
    }

    public void ResetDestroy()
    {
        _isDestroyed = false;
    }
}