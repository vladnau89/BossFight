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
            EditorGUILayout.PropertyField(PropertyFromName("_turnRate"));
            EditorGUILayout.PropertyField(PropertyFromName("_maxHomingDistance"));
            EditorGUILayout.PropertyField(PropertyFromName("_maxHomingConeAngle"));
            EditorGUILayout.PropertyField(PropertyFromName("_maxDeflectionAngle"));
            EditorGUILayout.PropertyField(PropertyFromName("_stopHomingWhenTargetBehind"));
            EditorGUILayout.PropertyField(PropertyFromName("_rotateTowardsVelocity"));
            EditorGUI.indentLevel--;
        }

        base.DrawObjectFields();
    }
}
#endif
