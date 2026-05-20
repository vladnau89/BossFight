using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Per-phase weak point set: collider registration for combat + root/entity activation.
/// </summary>
[DisallowMultipleComponent]
public sealed class BossPhaseWeakPointsComponent : MonoBehaviour
{
    [SerializeField] private GameObject _weakPointsRoot;
    [SerializeField] private WeakPointEntity[] _weakPoints;
    [SerializeField] private bool _deactivateRootOnAwake;

    private void Awake()
    {
        if (_deactivateRootOnAwake) {
            SetRootActive(false);
        }
    }

    public void RegisterWeakPoints(Dictionary<Collider, WeakPointEntity> map)
    {
        foreach (var weakPoint in _weakPoints) {
            map[weakPoint.Collider] = weakPoint;
        }
    }

    public void SetActive(bool active)
    {
        SetRootActive(active);

        foreach (var weakPoint in _weakPoints) {
            weakPoint.ResetHealth();
            weakPoint.SetActive(active);
        }
    }

    public void SetRootActive(bool active)
    {
        _weakPointsRoot.SetActive(active);
    }
}
