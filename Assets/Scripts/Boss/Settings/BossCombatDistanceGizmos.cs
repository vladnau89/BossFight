using UnityEngine;

/// <summary>
/// Draws search/attack range wire spheres from <see cref="BossCombatSettings"/> when gizmo drawing is enabled.
/// </summary>
[DisallowMultipleComponent]
public sealed class BossCombatDistanceGizmos : MonoBehaviour
{
    private static readonly Color s_searchColor = new(0.2f, 0.85f, 1f, 0.9f);
    private static readonly Color s_attackColor = new(1f, 0.35f, 0.2f, 0.9f);

    [SerializeField] private BossCombatSettings _settings;

    private void OnDrawGizmos()
    {
        if (_settings == null || !_settings.DrawDistanceGizmos) {
            return;
        }

        var origin = transform.position + _settings.DistanceGizmoOriginOffset;

        DrawRangeGizmo(origin, _settings.SearchDistance, s_searchColor);
        DrawRangeGizmo(origin, _settings.AttackDistance, s_attackColor);
    }

    private static void DrawRangeGizmo(Vector3 origin, float radius, Color color)
    {
        if (radius <= 0f) {
            return;
        }

        Gizmos.color = color;
        Gizmos.DrawWireSphere(origin, radius);
    }
}
