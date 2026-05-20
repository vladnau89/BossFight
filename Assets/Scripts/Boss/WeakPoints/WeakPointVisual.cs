using UnityEngine;

/// <summary>
/// Glow pulse and hit feedback for weak point markers.
/// </summary>
[DisallowMultipleComponent]
public class WeakPointVisual : MonoBehaviour
{
    [SerializeField] private RenderPulse _pulse;
    [SerializeField] private RenderScale _scale;
    [SerializeField] private float _destroyFlashDuration = 0.25f;

    public float DestroyFlashDuration => _destroyFlashDuration;

    public void PlayHitFlash() => _pulse.PlayHitFlash();

    public void PlayDestroyed()
    {
        _scale.Play(_destroyFlashDuration);
        _pulse.PlayDestroyed(_destroyFlashDuration);
    }
}
