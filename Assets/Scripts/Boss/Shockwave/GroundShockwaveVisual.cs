using UnityEngine;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
public sealed class GroundShockwaveVisual : MonoBehaviour
{
    private static Material s_RingMaterial;

    [SerializeField] private LineRenderer _outerRing;
    [SerializeField] private LineRenderer _innerRing;
    [SerializeField] private int _segments = 48;
    [SerializeField] private float _visualHeightOffset = 0.08f;
    [SerializeField] private float _outerLineWidth = 0.45f;
    [SerializeField] private float _innerLineWidth = 0.25f;
    [SerializeField] private Color _color = new Color(1f, 0.5f, 0.15f, 0.9f);

    private Vector3[] _ringPoints;

    private void Awake() => EnsureReady();

    public void EnsureReady()
    {
        if (_outerRing == null) {
            _outerRing = CreateRingLineRenderer("OuterRing", _outerLineWidth);
        } else {
            ConfigureRingLineRenderer(_outerRing, _outerLineWidth);
        }

        if (_innerRing == null) {
            _innerRing = CreateRingLineRenderer("InnerRing", _innerLineWidth);
        } else {
            ConfigureRingLineRenderer(_innerRing, _innerLineWidth);
        }
    }

    public void UpdateRing(float radius, float maxRadius, float width)
    {
        var fade = 1f - Mathf.Clamp01(radius / maxRadius);
        var color = _color;
        color.a *= Mathf.Lerp(0.2f, 1f, fade);

        SetRingPositions(_outerRing, radius, color);
        SetRingPositions(_innerRing, Mathf.Max(0.05f, radius - width), color * 0.65f);
    }

    private LineRenderer CreateRingLineRenderer(string objectName, float width)
    {
        var go = new GameObject(objectName);
        go.transform.SetParent(transform, false);

        var lineRenderer = go.AddComponent<LineRenderer>();
        ConfigureRingLineRenderer(lineRenderer, width);
        return lineRenderer;
    }

    private void ConfigureRingLineRenderer(LineRenderer lineRenderer, float width)
    {
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

    private void SetRingPositions(LineRenderer lineRenderer, float radius, Color color)
    {
        var pointCount = _segments + 1;
        if (_ringPoints == null || _ringPoints.Length != pointCount) {
            _ringPoints = new Vector3[pointCount];
        }

        var y = _visualHeightOffset;
        for (var i = 0; i < pointCount; i++) {
            var angle = i / (float)_segments * Mathf.PI * 2f;
            _ringPoints[i] = new Vector3(Mathf.Cos(angle) * radius, y, Mathf.Sin(angle) * radius);
        }

        lineRenderer.positionCount = pointCount;
        lineRenderer.SetPositions(_ringPoints);
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }
}
