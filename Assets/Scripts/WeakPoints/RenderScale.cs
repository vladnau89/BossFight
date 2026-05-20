using UnityEngine;

[DisallowMultipleComponent]
public class RenderScale : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _duration = 0.25f;

    private float _timer;
    private float _currentDuration;
    private Vector3 _baseScale;

    private void Awake()
    {
        _baseScale = _target.localScale;
    }

    private void OnEnable()
    {
        _timer = 0f;
        if (_baseScale.sqrMagnitude > 0f) {
            _target.localScale = _baseScale;
        }
    }

    private void Update()
    {
        _timer = Mathf.Max(0f, _timer - Time.deltaTime);
        if (_timer <= 0f) {
            return;
        }

        var t = 1f - (_timer / _currentDuration);
        var scale = Mathf.Lerp(_baseScale.x * 1.6f, 0.01f, t);
        _target.localScale = _baseScale * (scale / _baseScale.x);
    }

    public void Play(float durationSeconds)
    {
        _currentDuration = durationSeconds > 0f ? durationSeconds : _duration;
        _timer = _currentDuration;
    }

    public void Play() => Play(_duration);

    public void ResetScale()
    {
        _timer = 0f;
        if (_baseScale.sqrMagnitude > 0f) {
            _target.localScale = _baseScale;
        }
    }
}

