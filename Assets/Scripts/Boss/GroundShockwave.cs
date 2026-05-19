using Opsive.UltimateCharacterController.Game;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Expanding ground damage ring. Spawned by boss attack components.
/// </summary>
public class GroundShockwave : MonoBehaviour
{
    private static Material s_RingMaterial;

    [SerializeField] private float m_Speed = 12f;
    [SerializeField] private float m_Width = 2f;
    [SerializeField] private float m_MaxRadius = 25f;
    [SerializeField] private float m_Damage = 15f;
    [SerializeField] private float m_ForceMagnitude = 2f;
    [SerializeField] private LayerMask m_TargetLayers = (1 << LayerManager.Character) | (1 << LayerManager.SubCharacter);

    [Header("Visual")]
    [SerializeField] private LineRenderer m_OuterRing;
    [SerializeField] private LineRenderer m_InnerRing;
    [SerializeField] private int m_Segments = 48;
    [SerializeField] private float m_VisualHeightOffset = 0.08f;
    [SerializeField] private float m_OuterLineWidth = 0.45f;
    [SerializeField] private float m_InnerLineWidth = 0.25f;
    [SerializeField] private Color m_Color = new Color(1f, 0.5f, 0.15f, 0.9f);

    private float m_Radius;
    private GameObject m_Attacker;
    private Vector3[] m_RingPoints;

    private void Awake()
    {
        EnsureVisual();
    }

    public void Initialize(Vector3 position, GameObject attacker, float damage, float maxRadius, float speed)
    {
        transform.position = position;
        m_Attacker = attacker;
        m_Damage = damage;
        m_MaxRadius = maxRadius;
        m_Speed = speed;
        m_Radius = m_Width;
        EnsureVisual();
        UpdateVisual();
    }

    private void Update()
    {
        var previousRadius = m_Radius;
        m_Radius += m_Speed * Time.deltaTime;
        if (m_Radius >= m_MaxRadius) {
            Destroy(gameObject);
            return;
        }

        AreaDamageUtility.DamageRing(transform.position, previousRadius, m_Radius, m_Damage, m_ForceMagnitude, m_Attacker, m_TargetLayers, requireGrounded: true);
        UpdateVisual();
    }

    private void EnsureVisual()
    {
        if (m_OuterRing == null) {
            m_OuterRing = CreateRingLineRenderer("OuterRing", m_OuterLineWidth);
        }
        if (m_InnerRing == null) {
            m_InnerRing = CreateRingLineRenderer("InnerRing", m_InnerLineWidth);
        }
    }

    private LineRenderer CreateRingLineRenderer(string objectName, float width)
    {
        var go = new GameObject(objectName);
        go.transform.SetParent(transform, false);

        var lineRenderer = go.gameObject.AddComponent<LineRenderer>();

        lineRenderer.loop = true;
        lineRenderer.useWorldSpace = false;
        lineRenderer.shadowCastingMode = ShadowCastingMode.Off;
        lineRenderer.receiveShadows = false;
        lineRenderer.textureMode = LineTextureMode.Stretch;
        lineRenderer.numCapVertices = 4;
        lineRenderer.numCornerVertices = 4;
        lineRenderer.material = GetRingMaterial();
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        return lineRenderer;
    }

    private static Material GetRingMaterial()
    {
        if (s_RingMaterial != null) {
            return s_RingMaterial;
        }

        var shader = Shader.Find("Legacy Shaders/Particles/Additive");
        if (shader == null) {
            shader = Shader.Find("Sprites/Default");
        }
        s_RingMaterial = new Material(shader);
        return s_RingMaterial;
    }

    private void UpdateVisual()
    {
        if (m_OuterRing == null) {
            return;
        }

        var fade = 1f - Mathf.Clamp01(m_Radius / m_MaxRadius);
        var color = m_Color;
        color.a *= Mathf.Lerp(0.2f, 1f, fade);

        SetRingPositions(m_OuterRing, m_Radius, color);
        SetRingPositions(m_InnerRing, Mathf.Max(0.05f, m_Radius - m_Width), color * 0.65f);
    }

    private void SetRingPositions(LineRenderer lineRenderer, float radius, Color color)
    {
        if (lineRenderer == null) {
            return;
        }

        var pointCount = m_Segments + 1;
        if (m_RingPoints == null || m_RingPoints.Length != pointCount) {
            m_RingPoints = new Vector3[pointCount];
        }

        var y = m_VisualHeightOffset;
        for (var i = 0; i < pointCount; i++) {
            var angle = i / (float)m_Segments * Mathf.PI * 2f;
            m_RingPoints[i] = new Vector3(Mathf.Cos(angle) * radius, y, Mathf.Sin(angle) * radius);
        }

        lineRenderer.positionCount = pointCount;
        lineRenderer.SetPositions(m_RingPoints);
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.35f);
        Gizmos.DrawWireSphere(transform.position, m_Radius);
    }
}
