using UnityEngine;

[DisallowMultipleComponent]
public sealed class DestroyVisualComponent : MonoBehaviour
{
    [SerializeField] private float _hitFlashDuration = 0.12f;
    [SerializeField] private float _hitFlashEmission = 6f;
    [SerializeField] private float _destroyFlashDuration = 0.25f;

    public float HitFlashDuration => _hitFlashDuration;
    public float HitFlashEmission => _hitFlashEmission;
    public float DestroyFlashDuration => _destroyFlashDuration;
    public float DestroyFlashEmission => _hitFlashEmission * 1.5f;

    public float GetEmissionForDuration(float duration)
    {
        return Mathf.Approximately(duration, _hitFlashDuration)
            ? _hitFlashEmission
            : DestroyFlashEmission;
    }
}
