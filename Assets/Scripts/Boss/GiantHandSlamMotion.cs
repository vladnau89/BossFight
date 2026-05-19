using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Raises and lowers the giant hand on the boss skeleton (child of the hand bone), facing the player.
/// </summary>
public class GiantHandSlamMotion : MonoBehaviour
{
    [SerializeField] private Vector3 m_RaiseLocalEuler = new Vector3(-80f, 0f, 0f);
    [SerializeField] private Vector3 m_SlamArmEuler = new Vector3(35f, 0f, 0f);
    [SerializeField] private Vector3 m_RotationOffsetEuler = new Vector3(-90f, 0f, 0f);
    [SerializeField] private Vector3 m_SlamReachLocalOffset = new Vector3(0f, -0.15f, 0.6f);
    [SerializeField] private float m_RaiseTime = 0.35f;
    [SerializeField] private float m_HoverTime = 0.25f;
    [SerializeField] private float m_DropTime = 0.5f;
    [SerializeField] private float m_HoldAfterImpactTime = 0.15f;
    [SerializeField] private bool m_HideAfterSlam = true;

    private Vector3 m_RestLocalPosition;
    private Quaternion m_RestLocalRotation;
    private bool m_IsPlaying;

    public bool IsPlaying => m_IsPlaying;

    private void Awake()
    {
        CacheRestPose();
    }

    public void Play(Transform target, Action onImpact)
    {
        if (m_IsPlaying || target == null) {
            return;
        }
        StartCoroutine(SlamRoutine(target, onImpact));
    }

    public void CancelAndRestore()
    {
        StopAllCoroutines();
        m_IsPlaying = false;
        RestoreRestPose();
    }

    public void CacheRestPose()
    {
        m_RestLocalPosition = transform.localPosition;
        m_RestLocalRotation = transform.localRotation;
    }

    private IEnumerator SlamRoutine(Transform target, Action onImpact)
    {
        m_IsPlaying = true;
        gameObject.SetActive(true);
        transform.localPosition = m_RestLocalPosition;
        transform.localRotation = m_RestLocalRotation;

        var raisedRotation = m_RestLocalRotation * Quaternion.Euler(m_RaiseLocalEuler);

        Debug.Log("Start Raise Hand!");
        if (m_RaiseTime > 0f) {
            var elapsed = 0f;
            while (elapsed < m_RaiseTime) {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / m_RaiseTime);
                transform.localRotation = Quaternion.Slerp(m_RestLocalRotation, raisedRotation, t);
                yield return null;
            }
            transform.localRotation = raisedRotation;
        } else {
            transform.localRotation = raisedRotation;
        }

        Debug.Log("Start Hover Hand!");
        
        if (m_HoverTime > 0f) {
            yield return new WaitForSeconds(m_HoverTime);
        }

        Debug.Log("Start Drop Hand!");
        if (m_DropTime > 0f) {
            var elapsed = 0f;
            var startRotation = transform.rotation;
            var startLocalPosition = transform.localPosition;
            var endLocalPosition = m_RestLocalPosition + m_SlamReachLocalOffset;
            while (elapsed < m_DropTime) {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / m_DropTime);
                var eased = t * t;
                var slamRotation = GetSlamWorldRotation(target, eased);
                transform.rotation = Quaternion.Slerp(startRotation, slamRotation, eased);
                transform.localPosition = Vector3.Lerp(startLocalPosition, endLocalPosition, eased);
                yield return null;
            }
        }

        transform.rotation = GetSlamWorldRotation(target, 1f);
        transform.localPosition = m_RestLocalPosition + m_SlamReachLocalOffset;
        onImpact?.Invoke();

        if (m_HoldAfterImpactTime > 0f) {
            yield return new WaitForSeconds(m_HoldAfterImpactTime);
        }

        RestoreRestPose();
        m_IsPlaying = false;
    }

    private Quaternion GetSlamWorldRotation(Transform target, float dropT)
    {
        var toPlayer = target.position - transform.position;
        toPlayer.y = 0f;
        if (toPlayer.sqrMagnitude < 0.0001f && transform.parent != null) {
            toPlayer = transform.parent.forward;
            toPlayer.y = 0f;
        }
        if (toPlayer.sqrMagnitude < 0.0001f) {
            toPlayer = Vector3.forward;
        }

        var pitch = Mathf.Lerp(m_RaiseLocalEuler.x, m_SlamArmEuler.x, dropT);
        var yawToPlayer = Quaternion.LookRotation(toPlayer.normalized, Vector3.up);
        return yawToPlayer * Quaternion.Euler(pitch + m_RotationOffsetEuler.x, m_RotationOffsetEuler.y, m_RotationOffsetEuler.z);
    }

    private void RestoreRestPose()
    {
        transform.localPosition = m_RestLocalPosition;
        transform.localRotation = m_RestLocalRotation;
        if (m_HideAfterSlam) {
            gameObject.SetActive(false);
        }
    }
}
