using UnityEngine;

[DisallowMultipleComponent]
public class RenderPulse : MonoBehaviour
{
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

    [SerializeField] private Renderer _renderer;
    [SerializeField] private float _pulseSpeed = 4f;
    [SerializeField] private float _minEmission = 1.2f;
    [SerializeField] private float _maxEmission = 3.5f;
    [SerializeField] private Color _emissionColor = new(1f, 0.4f, 0.05f);
    [SerializeField] private float _hitFlashDuration = 0.12f;
    [SerializeField] private float _hitFlashEmission = 6f;

    private MaterialPropertyBlock _propertyBlock;
    private float _hitFlashTimer;
    private float _destroyFlashTimer;

    private void Awake()
    {
        _propertyBlock = new MaterialPropertyBlock();
    }

    private void OnEnable()
    {
        _hitFlashTimer = 0f;
        _destroyFlashTimer = 0f;
    }

    private void Update()
    {
        _hitFlashTimer = Mathf.Max(0f, _hitFlashTimer - Time.deltaTime);
        _destroyFlashTimer = Mathf.Max(0f, _destroyFlashTimer - Time.deltaTime);

        var emissionScale = _minEmission +
                            (Mathf.Sin(Time.time * _pulseSpeed) * 0.5f + 0.5f) *
                            (_maxEmission - _minEmission);

        if (_hitFlashTimer > 0f) {
            emissionScale = _hitFlashEmission;
        } else if (_destroyFlashTimer > 0f) {
            emissionScale = _hitFlashEmission * 1.5f;
        }

        _renderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetColor(EmissionColorId, _emissionColor * emissionScale);
        _renderer.SetPropertyBlock(_propertyBlock);
    }

    public void PlayHitFlash()
    {
        _hitFlashTimer = _hitFlashDuration;
    }

    public void PlayDestroyed(float durationSeconds)
    {
        _destroyFlashTimer = durationSeconds;
        _hitFlashTimer = 0f;
    }

    public void ResetPulse()
    {
        _hitFlashTimer = 0f;
        _destroyFlashTimer = 0f;
        _renderer.SetPropertyBlock(null);
    }
}

