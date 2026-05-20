using UnityEngine;

/// <summary>
/// Glow pulse and hit feedback for weak point markers.
/// </summary>
[DisallowMultipleComponent]
public class WeakPointVisual : MonoBehaviour
{
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

    [SerializeField] private Renderer _renderer;
    [SerializeField] private float _pulseSpeed = 4f;
    [SerializeField] private float _minEmission = 1.2f;
    [SerializeField] private float _maxEmission = 3.5f;
    [SerializeField] private Color _emissionColor = new Color(1f, 0.4f, 0.05f);
    [SerializeField] private float _hitFlashDuration = 0.12f;
    [SerializeField] private float _hitFlashEmission = 6f;
    [SerializeField] private float _destroyFlashDuration = 0.25f;

    private MaterialPropertyBlock _propertyBlock;
    private float _hitFlashTimer;
    private float _destroyFlashTimer;
    private Vector3 _baseScale;

    public float DestroyFlashDuration => _destroyFlashDuration;

    private void Awake()
    {
        _propertyBlock = new MaterialPropertyBlock();
        _baseScale = transform.localScale;
    }

    private void OnEnable()
    {
        _hitFlashTimer = 0f;
        _destroyFlashTimer = 0f;
        if (_baseScale.sqrMagnitude > 0f) {
            transform.localScale = _baseScale;
        }
    }

    private void Update()
    {
        if (_renderer == null) {
            return;
        }

        _hitFlashTimer = Mathf.Max(0f, _hitFlashTimer - Time.deltaTime);
        _destroyFlashTimer = Mathf.Max(0f, _destroyFlashTimer - Time.deltaTime);

        var emissionScale = _minEmission + (Mathf.Sin(Time.time * _pulseSpeed) * 0.5f + 0.5f) * (_maxEmission - _minEmission);
        if (_hitFlashTimer > 0f) {
            emissionScale = _hitFlashEmission;
        } else if (_destroyFlashTimer > 0f) {
            emissionScale = _hitFlashEmission * 1.5f;
        }

        _renderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetColor(EmissionColorId, _emissionColor * emissionScale);
        _renderer.SetPropertyBlock(_propertyBlock);

        if (_destroyFlashTimer > 0f) {
            var t = 1f - (_destroyFlashTimer / _destroyFlashDuration);
            var scale = Mathf.Lerp(_baseScale.x * 1.6f, 0.01f, t);
            transform.localScale = _baseScale * (scale / _baseScale.x);
        }
    }

    public void PlayHitFlash()
    {
        _hitFlashTimer = _hitFlashDuration;
    }

    public void PlayDestroyed()
    {
        _destroyFlashTimer = _destroyFlashDuration;
        _hitFlashTimer = 0f;
    }

    public void ResetVisual()
    {
        _hitFlashTimer = 0f;
        _destroyFlashTimer = 0f;
        if (_baseScale.sqrMagnitude > 0f) {
            transform.localScale = _baseScale;
        }
        if (_renderer != null) {
            _renderer.SetPropertyBlock(null);
        }
    }
}
