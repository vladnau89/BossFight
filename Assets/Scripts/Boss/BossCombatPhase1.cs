using System.Collections;
using Opsive.UltimateCharacterController.Game;
using Opsive.UltimateCharacterController.Traits;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Phase 1: ranged combat, giant hand slam, hand weak points.
/// </summary>
[DisallowMultipleComponent]
public class BossCombatPhase1 : BossCombatPhase
{
    [Header("Phase 1 objects")]
    [SerializeField] private GameObject m_GiantHandRoot;
    [SerializeField] private GameObject m_RangedWeaponRoot;

    [Header("Hand slam")]
    [SerializeField] private GiantHandSlamMotion m_HandSlamMotion;
    [SerializeField] private GiantHandSlamDamage m_HandSlamDamage;
    [SerializeField] private GroundShockwave m_ShockwavePrefab;
    [SerializeField] private Transform m_SlamOrigin;
    [SerializeField] private float m_HandSlamDamageAmount = 35f;
    [SerializeField] private float m_HandSlamForce = 4f;
    [SerializeField] private float m_WaveDelayMin = 1f;
    [SerializeField] private float m_WaveDelayMax = 3f;
    [SerializeField] private float m_WaveDamage = 12f;
    [SerializeField] private float m_WaveMaxRadius = 20f;
    [SerializeField] private float m_WaveSpeed = 10f;
    [SerializeField] private LayerMask m_PlayerDamageLayers = (1 << LayerManager.Character) | (1 << LayerManager.SubCharacter);
    [SerializeField] private BossCombat m_BossCombat;
    [SerializeField] private Health m_Health;

    public GameObject GiantHandRoot => m_GiantHandRoot;
    public override bool InProgress => m_HandSlamMotion != null && m_HandSlamMotion.IsPlaying;

    private void Awake()
    {
        if (m_GiantHandRoot != null) {
            m_GiantHandRoot.SetActive(false);
        }
    }


    public override void OnPhaseEnter()
    {
        ShowRanged();
    }

    public override void OnPhaseExit()
    {
        CancelHandSlam();
        if (m_GiantHandRoot != null) {
            m_GiantHandRoot.SetActive(false);
        }
        base.OnPhaseExit();
    }

    public void ShowRanged()
    {
        if (m_GiantHandRoot != null) {
            m_GiantHandRoot.SetActive(false);
        }
        if (m_RangedWeaponRoot != null) {
            m_RangedWeaponRoot.SetActive(true);
        }
    }

    public void ShowGiantHandPrep()
    {
        if (m_RangedWeaponRoot != null) {
            m_RangedWeaponRoot.SetActive(false);
        }
        if (m_GiantHandRoot != null) {
            m_GiantHandRoot.SetActive(false);
        }
    }

    public void PerformHandSlam(Transform target)
    {
        if (m_BossCombat != null && !m_BossCombat.IsActivePhase<BossCombatPhase1>()) {
            return;
        }
        if (InProgress || m_GiantHandRoot == null || m_HandSlamMotion == null || target == null) {
            return;
        }
        
        SetWeakPointsActive(false);
        m_GiantHandRoot.SetActive(true);
        m_HandSlamMotion.Play(target, OnHandSlamLanded);
    }

    public void CancelHandSlam()
    {
        StopAllCoroutines();
        if (m_HandSlamMotion != null) {
            m_HandSlamMotion.CancelAndRestore();
        }
    }

    private void OnHandSlamLanded()
    {
        if (m_BossCombat != null && !m_BossCombat.IsActivePhase<BossCombatPhase1>()) {
            return;
        }

        SetWeakPointsActive(true);
        ApplyHandSlamDamage();
        StartCoroutine(SpawnGroundShockwaveCoroutine());
    }

    private void ApplyHandSlamDamage()
    {
        if (m_Health == null || !m_Health.IsAlive() || m_HandSlamDamage == null) {
            return;
        }

        m_HandSlamDamage.BeginSlam(m_HandSlamDamageAmount, m_HandSlamForce, gameObject, m_PlayerDamageLayers);
    }

    private IEnumerator SpawnGroundShockwaveCoroutine()
    {
        yield return new WaitForSeconds(Random.Range(m_WaveDelayMin, m_WaveDelayMax));
        if (m_Health == null || !m_Health.IsAlive() ||
            (m_BossCombat != null && !m_BossCombat.IsActivePhase<BossCombatPhase1>())) {
            yield break;
        }

        SpawnWave(m_WaveDamage, m_WaveMaxRadius, m_WaveSpeed);
    }

    private void SpawnWave(float damage, float maxRadius, float speed)
    {
        if (m_ShockwavePrefab == null) {
            return;
        }

        var origin = m_SlamOrigin != null ? m_SlamOrigin.position : transform.position;
        var wave = Instantiate(m_ShockwavePrefab, origin, Quaternion.identity);
        wave.Initialize(origin, gameObject, damage, maxRadius, speed);
    }
}
