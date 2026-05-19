using System;
using System.Collections;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Game;
using Opsive.UltimateCharacterController.Traits;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Giant hand phase: slam AoE, ground wave, and weak points for the boss.
/// </summary>
[DefaultExecutionOrder(-100)]
public class BossCombat : MonoBehaviour
{
    [Header("Phase objects")]
    [SerializeField] private GameObject m_GiantHandRoot;
    [SerializeField] private GameObject m_RangedWeaponRoot;

    [Header("Slam")]
    [SerializeField] private Transform m_SlamOrigin;
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
    [SerializeField] private LayerMask m_PlayerDamageLayers = (1 << LayerManager.Character) | (1 << LayerManager.SubCharacter);

    [Header("Weak points")]
    [SerializeField] private WeakPointMarker[] m_WeakPoints;
    [SerializeField] private bool m_WeakPointsActive;

    private Health m_Health;

    private void Awake()
    {
        m_Health = GetComponent<Health>();
        if (m_WeakPoints == null || m_WeakPoints.Length == 0) {
            m_WeakPoints = m_GiantHandRoot != null
                ? m_GiantHandRoot.GetComponentsInChildren<WeakPointMarker>(true)
                : GetComponentsInChildren<WeakPointMarker>(true);
        }
        ClearWeakPointHitboxes();
        SetWeakPointsActive(m_WeakPointsActive);
        if (m_GiantHandRoot != null) {
            m_GiantHandRoot.SetActive(false);
        }
        if (m_GiantHandRoot != null) {
            if (m_HandSlamMotion == null) {
                m_HandSlamMotion = m_GiantHandRoot.GetComponent<GiantHandSlamMotion>();
            }
            if (m_HandSlamDamage == null) {
                m_HandSlamDamage = m_GiantHandRoot.GetComponentInChildren<GiantHandSlamDamage>(true);
            }
            if (m_SlamOrigin == null) {
                m_SlamOrigin = m_GiantHandRoot.transform.Find("SlamOrigin");
            }
        }
        
        m_Health.OnDeathEvent.AddListener(OnDeathEvent);
    }

    private void OnDestroy()
    {
        m_Health.OnDeathEvent.RemoveListener(OnDeathEvent);
    }

    private void OnDeathEvent(Vector3 arg0, Vector3 arg1, GameObject arg2)
    {
        StopAllCoroutines();
        m_HandSlamMotion.CancelAndRestore();
        ShowRangedPhase();
    }

    public bool IsHandSlamInProgress => m_HandSlamMotion != null && m_HandSlamMotion.IsPlaying;

    public void ShowRangedPhase()
    {
        if (m_GiantHandRoot != null) {
            m_GiantHandRoot.SetActive(false);
        }
        if (m_RangedWeaponRoot != null) {
            m_RangedWeaponRoot.SetActive(true);
        }
        SetWeakPointsActive(false);
    }

    public void ShowGiantHandPhase()
    {
        if (m_RangedWeaponRoot != null) {
            m_RangedWeaponRoot.SetActive(false);
        }
        if (m_GiantHandRoot != null) {
            m_GiantHandRoot.SetActive(false);
        }
    }

    public void SetWeakPointsActive(bool active)
    {
        m_WeakPointsActive = active;
        if (m_WeakPoints == null) {
            return;
        }

        for (var i = 0; i < m_WeakPoints.Length; i++) {
            if (m_WeakPoints[i] == null) {
                continue;
            }
            if (active) {
                m_WeakPoints[i].ResetHealth();
            }
            m_WeakPoints[i].gameObject.SetActive(active);
            var collider = m_WeakPoints[i].Collider;
            if (collider != null) {
                collider.enabled = active && !m_WeakPoints[i].IsDestroyed;
            }
        }
    }

    public bool IsWeakPointCollider(Collider hitCollider)
    {
        if (m_WeakPoints == null || hitCollider == null) {
            return false;
        }

        for (var i = 0; i < m_WeakPoints.Length; i++) {
            var weakPoint = m_WeakPoints[i];
            if (weakPoint != null && weakPoint.Collider == hitCollider) {
                return true;
            }
        }

        return false;
    }

    public bool TryDamageWeakPoint(Collider hitCollider, float amount, Vector3 position, Vector3 direction, float forceMagnitude, int frames, GameObject attacker, object attackerObject)
    {
        if (!m_WeakPointsActive || m_WeakPoints == null || hitCollider == null) {
            return false;
        }

        for (var i = 0; i < m_WeakPoints.Length; i++) {
            var weakPoint = m_WeakPoints[i];
            if (weakPoint == null || weakPoint.Collider != hitCollider) {
                continue;
            }
            return weakPoint.TakeDamage(amount, position, direction, forceMagnitude, frames, attacker, attackerObject, hitCollider);
        }

        return false;
    }

    public void ApplyWeakPointBurstDamage(float amount, Vector3 position, Vector3 direction, GameObject attacker, object attackerObject)
    {
        if (m_Health is BossCharacterHealth bossHealth) {
            bossHealth.ApplyWeakPointBurstDamage(amount, position, direction, attacker, attackerObject);
        }
    }

    public void OnWeakPointDestroyed(WeakPointMarker weakPoint)
    {
        ClearWeakPointHitboxes();
    }

    public void PerformHandSlam(Transform target)
    {
        if (m_HandSlamMotion != null && m_HandSlamMotion.IsPlaying) {
            return;
        }
        
        if (m_HandSlamMotion != null && target != null) {
            if (m_GiantHandRoot != null) {
                m_GiantHandRoot.SetActive(true);
            }
            m_HandSlamMotion.Play(target, OnHandSlamLanded);
            return;
        }

        OnHandSlamLanded();
    }

    private void OnHandSlamLanded()
    {
        SetWeakPointsActive(true);
        ApplyHandSlamDamage();
        StartCoroutine(SpawnGroundShockwaveCoroutine());
    }

    private void ApplyHandSlamDamage()
    {
        if (!m_Health.IsAlive() || m_HandSlamDamage == null) {
            return;
        }

        m_HandSlamDamage.BeginSlam(m_HandSlamDamageAmount, m_HandSlamForce, gameObject, m_PlayerDamageLayers);
    }

    private IEnumerator SpawnGroundShockwaveCoroutine()
    {
        yield return new WaitForSeconds(Random.Range(m_WaveDelayMin, m_WaveDelayMax));
        if (!m_Health.IsAlive() || m_ShockwavePrefab == null) {
            yield break;
        }

        var origin = m_SlamOrigin != null ? m_SlamOrigin.position : transform.position;
        var wave = Instantiate(m_ShockwavePrefab, origin, Quaternion.identity);
        wave.Initialize(origin, gameObject, m_WaveDamage, m_WaveMaxRadius, m_WaveSpeed);
    }

    public void ClearWeakPointHitboxes()
    {
        if (m_Health != null) {
            m_Health.Hitboxes = System.Array.Empty<Hitbox>();
        }
    }
}
