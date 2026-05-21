using System;
using UnityEngine;

[Serializable]
public struct BossPhaseEnterBinding
{
    [SerializeField] private int _phaseIndex;
    [SerializeField] private BossPhaseEnterCondition _enterCondition;

    public int PhaseIndex => _phaseIndex;
    public BossPhaseEnterCondition EnterCondition => _enterCondition;
}
