using UnityEngine;

/// <summary>
/// Updates ring geometry at runtime. Line width, material, and color are configured on the prefab.
/// </summary>
[DisallowMultipleComponent]
public sealed class GroundShockwaveVisual : MonoBehaviour
{
    [SerializeField] private LineRenderer _ring;
    [SerializeField] private int _segments = 48;
    [SerializeField] private float _visualHeightOffset = 0.08f;

    private Vector3[] _ringPoints;

    public void UpdateRing(float radius)
    {
        if (_ring == null) {
            return;
        }

        var pointCount = _segments + 1;
        if (_ringPoints == null || _ringPoints.Length != pointCount) {
            _ringPoints = new Vector3[pointCount];
        }

        var y = _visualHeightOffset;
        for (var i = 0; i < pointCount; i++) {
            var angle = i / (float)_segments * Mathf.PI * 2f;
            _ringPoints[i] = new Vector3(Mathf.Cos(angle) * radius, y, Mathf.Sin(angle) * radius);
        }

        _ring.positionCount = pointCount;
        _ring.SetPositions(_ringPoints);
    }
}
