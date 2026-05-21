using System;
using UnityEngine;

[Serializable]
public struct BossCombatHandSlamMotionSettings
{
    [Tooltip("Wind-up: time to raise the hand before hovering.")]
    [SerializeField] private float _raiseTime;

    [Tooltip("Pause at the top of the wind-up before the slam drop.")]
    [SerializeField] private float _hoverTime;

    [Tooltip("Time to slam the hand down toward the target.")]
    [SerializeField] private float _dropTime;

    public float RaiseTime => _raiseTime;
    public float HoverTime => _hoverTime;
    public float DropTime => _dropTime;

    public static BossCombatHandSlamMotionSettings Default => new BossCombatHandSlamMotionSettings
    {
        _raiseTime = 3f,
        _hoverTime = 3f,
        _dropTime = 1f,
    };
}
