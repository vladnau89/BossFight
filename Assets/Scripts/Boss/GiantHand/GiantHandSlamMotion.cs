using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Raises and lowers the giant hand on the boss skeleton (child of the hand bone), facing the player.
/// </summary>
public class GiantHandSlamMotion : MonoBehaviour
{
    [SerializeField] private Vector3 _raiseLocalEuler = new Vector3(-80f, 0f, 0f);
    [SerializeField] private Vector3 _slamArmEuler = new Vector3(35f, 0f, 0f);
    [SerializeField] private Vector3 _rotationOffsetEuler = new Vector3(-90f, 0f, 0f);
    [SerializeField] private Vector3 _slamReachLocalOffset = new Vector3(0f, -0.15f, 0.6f);
    [SerializeField] private float _raiseTime = 0.35f;
    [SerializeField] private float _hoverTime = 0.25f;
    [SerializeField] private float _dropTime = 0.5f;
    [SerializeField] private float _holdAfterImpactTime = 0.15f;
    [SerializeField] private bool _hideAfterSlam = true;

    private Vector3 _restLocalPosition;
    private Quaternion _restLocalRotation;
    private bool _isPlaying;

    public event Action PlayStarted;
    public event Action PlayFinished;

    public bool IsPlaying => _isPlaying;

    public void ApplyTimingSettings(float raiseTime, float hoverTime, float dropTime)
    {
        _raiseTime = raiseTime;
        _hoverTime = hoverTime;
        _dropTime = dropTime;
    }

    private void Awake() => CacheRestPose();

    public void Play(Transform target, Action onImpact)
    {
        if (_isPlaying || target == null) {
            return;
        }

        PlayStarted?.Invoke();
        StartCoroutine(SlamRoutine(target, onImpact));
    }

    public void CancelAndRestore()
    {
        StopAllCoroutines();
        _isPlaying = false;
        RestoreRestPose();
    }

    private void CacheRestPose()
    {
        _restLocalPosition = transform.localPosition;
        _restLocalRotation = transform.localRotation;
    }

    private IEnumerator SlamRoutine(Transform target, Action onImpact)
    {
        _isPlaying = true;
        gameObject.SetActive(true);
        transform.localPosition = _restLocalPosition;
        transform.localRotation = _restLocalRotation;

        var raisedRotation = _restLocalRotation * Quaternion.Euler(_raiseLocalEuler);

        if (_raiseTime > 0f) {
            var elapsed = 0f;
            while (elapsed < _raiseTime) {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / _raiseTime);
                transform.localRotation = Quaternion.Slerp(_restLocalRotation, raisedRotation, t);
                yield return null;
            }
            transform.localRotation = raisedRotation;
        } else {
            transform.localRotation = raisedRotation;
        }

        if (_hoverTime > 0f) {
            yield return new WaitForSeconds(_hoverTime);
        }

        if (_dropTime > 0f) {
            var elapsed = 0f;
            var startRotation = transform.rotation;
            var startLocalPosition = transform.localPosition;
            var endLocalPosition = _restLocalPosition + _slamReachLocalOffset;
            while (elapsed < _dropTime) {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / _dropTime);
                var eased = t * t;
                var slamRotation = GetSlamWorldRotation(target, eased);
                transform.rotation = Quaternion.Slerp(startRotation, slamRotation, eased);
                transform.localPosition = Vector3.Lerp(startLocalPosition, endLocalPosition, eased);
                yield return null;
            }
        }

        transform.rotation = GetSlamWorldRotation(target, 1f);
        transform.localPosition = _restLocalPosition + _slamReachLocalOffset;
        PlayFinished?.Invoke();
        onImpact?.Invoke();

        if (_holdAfterImpactTime > 0f) {
            yield return new WaitForSeconds(_holdAfterImpactTime);
        }

        RestoreRestPose();
        _isPlaying = false;
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

        var pitch = Mathf.Lerp(_raiseLocalEuler.x, _slamArmEuler.x, dropT);
        var yawToPlayer = Quaternion.LookRotation(toPlayer.normalized, Vector3.up);
        return yawToPlayer * Quaternion.Euler(pitch + _rotationOffsetEuler.x, _rotationOffsetEuler.y, _rotationOffsetEuler.z);
    }

    private void RestoreRestPose()
    {
        transform.localPosition = _restLocalPosition;
        transform.localRotation = _restLocalRotation;
        if (_hideAfterSlam) {
            gameObject.SetActive(false);
        }
    }
}
