#if UNITY_EDITOR
using Opsive.UltimateCharacterController.Editor.Inspectors.Objects;
using UnityEditor;

/// <summary>
/// Extends the Opsive Projectile inspector so homing fields are visible.
/// </summary>
[CustomEditor(typeof(HomingProjectile))]
[CanEditMultipleObjects]
public class HomingProjectileInspector : ProjectileInspector
{
    protected override void DrawObjectFields()
    {
        if (Foldout("Homing")) {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(PropertyFromName("m_TurnRate"));
            EditorGUILayout.PropertyField(PropertyFromName("m_MaxHomingDistance"));
            EditorGUILayout.PropertyField(PropertyFromName("m_MaxHomingConeAngle"));
            EditorGUILayout.PropertyField(PropertyFromName("m_MaxDeflectionAngle"));
            EditorGUILayout.PropertyField(PropertyFromName("m_StopHomingWhenTargetBehind"));
            EditorGUILayout.PropertyField(PropertyFromName("m_RotateTowardsVelocity"));
            EditorGUI.indentLevel--;
        }

        base.DrawObjectFields();
    }
}
#endif
