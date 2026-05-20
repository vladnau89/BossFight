using System.Collections;
using UnityEngine;

/// <summary>
/// Observes <see cref="WeakPointHealth"/> and drives <see cref="WeakPointVisual"/>; hides the object after destroy flash.
/// </summary>
[DisallowMultipleComponent]
public class WeakPointHealthChangedObserver_UpdateVisual : MonoBehaviour
{
    [SerializeField] private WeakPointHealth _health;
    [SerializeField] private WeakPointVisual _visual;
    [SerializeField] private WeakPointEntity _entity;

    private Coroutine _hideCoroutine;

    private void OnEnable()
    {
        _health.OnHealthChanged += HandleHealthChanged;
    }

    private void OnDisable()
    {
        _health.OnHealthChanged -= HandleHealthChanged;
    }

    public void CancelHide()
    {
        StopCoroutine(_hideCoroutine);
        _hideCoroutine = null;
    }

    private void HandleHealthChanged(WeakPointHealthChange change)
    {
        if (change.WasReset) {
            if (_hideCoroutine != null) {
                CancelHide();
            }
            _entity.SetActive(true);
            _visual.ResetVisual();
            return;
        }

        if (change.WasDestroyed) {
            _visual.PlayDestroyed();
            if (_hideCoroutine != null) {
                CancelHide();
            }
            _hideCoroutine = StartCoroutine(HideAfterDestroyFlash());
            return;
        }

        if (change.WasHit) {
            _visual.PlayHitFlash();
        }
    }

    private IEnumerator HideAfterDestroyFlash()
    {
        yield return new WaitForSeconds(_visual.DestroyFlashDuration);
        _entity.SetActive(false);
        _hideCoroutine = null;
    }
}
