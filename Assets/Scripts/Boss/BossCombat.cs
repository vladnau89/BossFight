using System;
using System.Collections;
using System.Collections.Generic;
using Opsive.UltimateCharacterController.Game;
using Opsive.UltimateCharacterController.Traits;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Boss combat phases: phase 1 giant hand slam + hand weak points; phase 2 chest cores + shock pulse.
/// </summary>
[DefaultExecutionOrder(-100)]
public class BossCombat : MonoBehaviour
{
    
    public enum CombatPhase
    {
        Phase1,
        Phase2
    }

    [Header("Phase objects")] [SerializeField]
    private GameObject m_GiantHandRoot;

    [SerializeField] private GameObject m_RangedWeaponRoot;
    [SerializeField] private GameObject m_Phase2WeakPointsRoot;

    [Header("Phase transition")] [SerializeField] [Range(0.05f, 0.95f)]
    private float m_Phase2HealthFraction = 0.5f;

    [Header("Slam")] [SerializeField] private Transform m_SlamOrigin;
    [SerializeField] private GiantHandSlamMotion m_HandSlamMotion;
    [SerializeField] private GiantHandSlamDamage m_HandSlamDamage;
    [SerializeField] private GroundShockwave m_ShockwavePrefab;
    [SerializeField] private float m_HandSlamDamageAmount = 35f;
    [SerializeField] private float m_HandSlamForce = 4f;
    [SerializeField] private float m_WaveDelayMin = 1f;
    [SerializeField] private float m_WaveDelayMax = 3f;
    [SerializeField] private float m_WaveDamage = 12f;
    [SerializeField] private float m_WaveMaxRadius = 20f;
    [SerializeField] private float m_WaveSpeed = 10f;

    [SerializeField]
    private LayerMask m_PlayerDamageLayers = (1 << LayerManager.Character) | (1 << LayerManager.SubCharacter);

    [Header("Phase 2 pulse")] [SerializeField]
    private Transform m_Phase2PulseOrigin;

    [SerializeField] private float m_Phase2PulseWindup = 0.6f;
    [SerializeField] private float m_Phase2PulseDamage = 18f;
    [SerializeField] private float m_Phase2PulseMaxRadius = 14f;
    [SerializeField] private float m_Phase2PulseSpeed = 12f;
    [SerializeField] private float m_Phase2VulnerabilityDuration = 8f;

    [Header("Weak points")] [SerializeField]
    private WeakPointMarker[] m_Phase1WeakPoints;

    [SerializeField] private WeakPointMarker[] m_Phase2WeakPoints;

    private Health m_Health;
    private CombatPhase m_CombatPhase = CombatPhase.Phase1;
    private Coroutine m_ChestPulseCoroutine;

    public CombatPhase CurrentPhase => m_CombatPhase;
    public bool IsPhase2 => m_CombatPhase == CombatPhase.Phase2;
    public bool IsHandSlamInProgress => m_HandSlamMotion != null && m_HandSlamMotion.IsPlaying;
    public bool IsChestPulseInProgress => m_ChestPulseCoroutine != null;

    public event Action<CombatPhase> OnCombatPhaseChanged;

    private Dictionary<Collider, WeakPointMarker> _weakPointMarkersMap = new();

    private void Awake()
    {
        m_Health = GetComponent<Health>();

        CacheWeakPointArrays();
        ClearWeakPointHitboxes();
        ApplyWeakPointsState(m_Phase1WeakPoints, false);
        ApplyWeakPointsState(m_Phase2WeakPoints, false);

        foreach (var weakPoint in m_Phase1WeakPoints)
        {
            _weakPointMarkersMap.Add(weakPoint.Collider, weakPoint);
        }

        foreach (var weakPoint in m_Phase2WeakPoints)
        {
            _weakPointMarkersMap.Add(weakPoint.Collider, weakPoint);
        }

        if (m_GiantHandRoot != null)
        {
            m_GiantHandRoot.SetActive(false);
        }

        if (m_Phase2WeakPointsRoot != null)
        {
            m_Phase2WeakPointsRoot.SetActive(false);
        }

        if (m_GiantHandRoot != null)
        {
            if (m_HandSlamMotion == null)
            {
                m_HandSlamMotion = m_GiantHandRoot.GetComponent<GiantHandSlamMotion>();
            }

            if (m_HandSlamDamage == null)
            {
                m_HandSlamDamage = m_GiantHandRoot.GetComponentInChildren<GiantHandSlamDamage>(true);
            }

            if (m_SlamOrigin == null)
            {
                m_SlamOrigin = m_GiantHandRoot.transform.Find("SlamOrigin");
            }
        }

        if (m_Phase2PulseOrigin == null)
        {
            m_Phase2PulseOrigin = transform;
        }

        if (m_Health != null)
        {
            m_Health.OnDeathEvent.AddListener(OnDeathEvent);
        }
    }

    private void OnDestroy()
    {
        if (m_Health != null)
        {
            m_Health.OnDeathEvent.RemoveListener(OnDeathEvent);
        }
    }

    private void Update()
    {
        if (IsPhase2 || m_Health == null || !m_Health.IsAlive())
        {
            return;
        }

        if (GetHealthFraction() <= m_Phase2HealthFraction)
        {
            EnterPhase2();
        }
    }

    private void CacheWeakPointArrays()
    {
        if (m_Phase1WeakPoints == null || m_Phase1WeakPoints.Length == 0)
        {
            if (m_GiantHandRoot != null)
            {
                m_Phase1WeakPoints = m_GiantHandRoot.GetComponentsInChildren<WeakPointMarker>(true);
            }
        }

        if ((m_Phase2WeakPoints == null || m_Phase2WeakPoints.Length == 0) && m_Phase2WeakPointsRoot != null)
        {
            m_Phase2WeakPoints = m_Phase2WeakPointsRoot.GetComponentsInChildren<WeakPointMarker>(true);
        }
    }

    private float GetHealthFraction()
    {
        if (m_Health.HealthValue > 0f)
        {
            return m_Health.HealthValue / m_Health.HealthMaxValue;
        }

        return 1f;
    }

    public void EnterPhase2()
    {
        if (IsPhase2)
        {
            return;
        }
        
        StopAllCoroutines();
        m_ChestPulseCoroutine = null;
        if (m_HandSlamMotion != null)
        {
            m_HandSlamMotion.CancelAndRestore();
        }

        m_Phase2WeakPointsRoot.SetActive(true);
        
        ApplyWeakPointsState(m_Phase1WeakPoints, false);
        m_CombatPhase = CombatPhase.Phase2;
        ShowRangedPhase();
        OnCombatPhaseChanged?.Invoke(m_CombatPhase);
    }
    
    private void OnDeathEvent(Vector3 arg0, Vector3 arg1, GameObject arg2)
    {
        StopAllCoroutines();
        m_ChestPulseCoroutine = null;
        if (m_HandSlamMotion != null)
        {
            m_HandSlamMotion.CancelAndRestore();
        }

        ShowRangedPhase();

        ApplyWeakPointsState(m_Phase1WeakPoints, false);
        ApplyWeakPointsState(m_Phase2WeakPoints, false);

        m_CombatPhase = CombatPhase.Phase1;
    }

    public void ShowRangedPhase()
    {
        if (m_GiantHandRoot != null)
        {
            m_GiantHandRoot.SetActive(false);
        }

        if (m_RangedWeaponRoot != null)
        {
            m_RangedWeaponRoot.SetActive(true);
        }
        
        m_Phase2WeakPointsRoot.SetActive(false);
    }

    public void ShowGiantHandPhase()
    {
        if (IsPhase2)
        {
            return;
        }

        if (m_RangedWeaponRoot != null)
        {
            m_RangedWeaponRoot.SetActive(false);
        }

        if (m_GiantHandRoot != null)
        {
            m_GiantHandRoot.SetActive(false);
        }
        
        m_Phase2WeakPointsRoot.SetActive(false);
    }

    private static void ApplyWeakPointsState(WeakPointMarker[] weakPoints, bool active)
    {
        foreach (var weakPoint in weakPoints)
        {
            if (weakPoint == null)
            {
                continue;
            }
            
            weakPoint.ResetHealth();
            weakPoint.SetActive(active);
        }
    }

    public bool IsWeakPointCollider(Collider hitCollider) => TryGetWeakPoint(hitCollider, out _);

    public bool TryDamageWeakPoint(Collider hitCollider, float amount, Vector3 position, Vector3 direction,
        float forceMagnitude, int frames, GameObject attacker, object attackerObject)
    {
        if (!TryGetWeakPoint(hitCollider, out var weakPoint))
        {
            return false;
        }

        return weakPoint.TakeDamage(amount, position, direction, forceMagnitude, frames, attacker, attackerObject,
            hitCollider);
    }

    private bool TryGetWeakPoint(Collider hitCollider, out WeakPointMarker weakPoint) => _weakPointMarkersMap.TryGetValue(hitCollider, out weakPoint);

    public void ApplyWeakPointBurstDamage(float amount, Vector3 position, Vector3 direction, GameObject attacker,
        object attackerObject)
    {
        if (m_Health is BossCharacterHealth bossHealth)
        {
            bossHealth.ApplyWeakPointBurstDamage(amount, position, direction, attacker, attackerObject);
        }
    }

    public void OnWeakPointDestroyed(WeakPointMarker weakPoint)
    {
        ClearWeakPointHitboxes();
    }

    public void PerformHandSlam(Transform target)
    {
        if (IsPhase2 || (m_HandSlamMotion != null && m_HandSlamMotion.IsPlaying))
        {
            return;
        }

        if (m_HandSlamMotion != null && target != null)
        {
            if (m_GiantHandRoot != null)
            {
                m_GiantHandRoot.SetActive(true);
            }

            m_HandSlamMotion.Play(target, OnHandSlamLanded);
            return;
        }

        OnHandSlamLanded();
    }

    public void PerformChestPulse()
    {
        if (!IsPhase2 || !m_Health.IsAlive() || m_ChestPulseCoroutine != null)
        {
            return;
        }
        
        m_ChestPulseCoroutine = StartCoroutine(ChestPulseCoroutine());
    }

    private IEnumerator ChestPulseCoroutine()
    {
        yield return new WaitForSeconds(m_Phase2PulseWindup);
        if (!m_Health.IsAlive())
        {
            m_ChestPulseCoroutine = null;
            yield break;
        }

        m_Phase2WeakPointsRoot.SetActive(true);
        ApplyWeakPointsState(m_Phase2WeakPoints, true);
        SpawnPulseWave(m_Phase2PulseDamage, m_Phase2PulseMaxRadius, m_Phase2PulseSpeed);

        yield return new WaitForSeconds(m_Phase2VulnerabilityDuration);
        m_ChestPulseCoroutine = null;
    }

    private void OnHandSlamLanded()
    {
        if (IsPhase2)
        {
            Debug.LogError("OnHandSlamLanded Phase 2!!~!!");
            return;
        }

        ApplyWeakPointsState(m_Phase1WeakPoints, true);
        ApplyHandSlamDamage();
        StartCoroutine(SpawnGroundShockwaveCoroutine());
    }

    private void ApplyHandSlamDamage()
    {
        if (!m_Health.IsAlive() || m_HandSlamDamage == null)
        {
            return;
        }

        m_HandSlamDamage.BeginSlam(m_HandSlamDamageAmount, m_HandSlamForce, gameObject, m_PlayerDamageLayers);
    }

    private IEnumerator SpawnGroundShockwaveCoroutine()
    {
        yield return new WaitForSeconds(Random.Range(m_WaveDelayMin, m_WaveDelayMax));
        if (!m_Health.IsAlive() || IsPhase2)
        {
            yield break;
        }

        SpawnPulseWave(m_WaveDamage, m_WaveMaxRadius, m_WaveSpeed);
    }

    private void SpawnPulseWave(float damage, float maxRadius, float speed)
    {
        if (m_ShockwavePrefab == null)
        {
            return;
        }

        var origin = m_CombatPhase == CombatPhase.Phase2 && m_Phase2PulseOrigin != null
            ? m_Phase2PulseOrigin.position
            : m_SlamOrigin != null
                ? m_SlamOrigin.position
                : transform.position;
        var wave = Instantiate(m_ShockwavePrefab, origin, Quaternion.identity);
        wave.Initialize(origin, gameObject, damage, maxRadius, speed);
    }

    public void ClearWeakPointHitboxes()
    {
        if (m_Health != null)
        {
            m_Health.Hitboxes = System.Array.Empty<Hitbox>();
        }
    }
    
}