using UnityEngine;

/// <summary>
/// Glow pulse and hit feedback for weak point markers.
/// </summary>
[DisallowMultipleComponent]
public class WeakPointVisual : MonoBehaviour
{
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

    [SerializeField] private Renderer m_Renderer;
    [SerializeField] private float m_PulseSpeed = 4f;
    [SerializeField] private float m_MinEmission = 1.2f;
    [SerializeField] private float m_MaxEmission = 3.5f;
    [SerializeField] private Color m_EmissionColor = new Color(1f, 0.4f, 0.05f);
    [SerializeField] private float m_HitFlashDuration = 0.12f;
    [SerializeField] private float m_HitFlashEmission = 6f;
    [SerializeField] private float m_DestroyFlashDuration = 0.25f;

    private MaterialPropertyBlock m_PropertyBlock;
    private float m_HitFlashTimer;
    private float m_DestroyFlashTimer;
    private Vector3 m_BaseScale;

    private void Awake()
    {
        m_PropertyBlock = new MaterialPropertyBlock();
        m_BaseScale = transform.localScale;
    }

    private void OnEnable()
    {
        m_HitFlashTimer = 0f;
        m_DestroyFlashTimer = 0f;
        transform.localScale = m_BaseScale;
    }

    private void Update()
    {
        if (m_Renderer == null) {
            return;
        }

        m_HitFlashTimer = Mathf.Max(0f, m_HitFlashTimer - Time.deltaTime);
        m_DestroyFlashTimer = Mathf.Max(0f, m_DestroyFlashTimer - Time.deltaTime);

        var emissionScale = m_MinEmission + (Mathf.Sin(Time.time * m_PulseSpeed) * 0.5f + 0.5f) * (m_MaxEmission - m_MinEmission);
        if (m_HitFlashTimer > 0f) {
            emissionScale = m_HitFlashEmission;
        } else if (m_DestroyFlashTimer > 0f) {
            emissionScale = m_HitFlashEmission * 1.5f;
        }

        m_Renderer.GetPropertyBlock(m_PropertyBlock);
        m_PropertyBlock.SetColor(EmissionColorId, m_EmissionColor * emissionScale);
        m_Renderer.SetPropertyBlock(m_PropertyBlock);

        if (m_DestroyFlashTimer > 0f) {
            var t = 1f - (m_DestroyFlashTimer / m_DestroyFlashDuration);
            var scale = Mathf.Lerp(m_BaseScale.x * 1.6f, 0.01f, t);
            transform.localScale = m_BaseScale * (scale / m_BaseScale.x);
        }
    }

    public void PlayHitFlash()
    {
        m_HitFlashTimer = m_HitFlashDuration;
    }

    public float DestroyFlashDuration => m_DestroyFlashDuration;

    public void PlayDestroyed()
    {
        m_DestroyFlashTimer = m_DestroyFlashDuration;
        m_HitFlashTimer = 0f;
    }

    public void ResetVisual()
    {
        m_HitFlashTimer = 0f;
        m_DestroyFlashTimer = 0f;
        if (m_BaseScale.sqrMagnitude > 0f) {
            transform.localScale = m_BaseScale;
        }
        if (m_Renderer != null) {
            m_Renderer.SetPropertyBlock(null);
        }
    }
}
