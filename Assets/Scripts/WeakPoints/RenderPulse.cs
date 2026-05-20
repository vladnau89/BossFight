using UnityEngine;

[DisallowMultipleComponent]
public class RenderPulse : MonoBehaviour
{
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

    [SerializeField] private Renderer _renderer;
    [SerializeField] private DestroyVisualComponent _destroyVisual;
    [SerializeField] private float _pulseSpeed = 4f;
    [SerializeField] private float _minEmission = 1.2f;
    [SerializeField] private float _maxEmission = 3.5f;
    [SerializeField] private Color _emissionColor = new(1f, 0.4f, 0.05f);

    private MaterialPropertyBlock _propertyBlock;
    private float _flashTimer;
    private float _flashEmissionScale;

    private void Awake()
    {
        _propertyBlock = new MaterialPropertyBlock();
    }

    private void OnEnable()
    {
        _flashTimer = 0f;
        _flashEmissionScale = 0f;
    }

    private void Update()
    {
        _flashTimer = Mathf.Max(0f, _flashTimer - Time.deltaTime);

        var emissionScale = _minEmission +
                            (Mathf.Sin(Time.time * _pulseSpeed) * 0.5f + 0.5f) *
                            (_maxEmission - _minEmission);

        if (_flashTimer > 0f) {
            emissionScale = _flashEmissionScale;
        }

        _renderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetColor(EmissionColorId, _emissionColor * emissionScale);
        _renderer.SetPropertyBlock(_propertyBlock);
    }

    public void Play(float duration)
    {
        _flashTimer = duration;
        _flashEmissionScale = _destroyVisual.GetEmissionForDuration(duration);
    }

    public void ResetPulse()
    {
        _flashTimer = 0f;
        _flashEmissionScale = 0f;
        _renderer.SetPropertyBlock(null);
    }
}
