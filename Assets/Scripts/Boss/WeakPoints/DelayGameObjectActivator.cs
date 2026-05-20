using System;
using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class DelayGameObjectActivator : MonoBehaviour
{
    [SerializeField] private GameObjectActivator _activator;
    [SerializeField] private float _delaySec = 0.25f;
    [SerializeField] private bool _activateAfterDelay = false;
    
    private Coroutine _delayCoroutine;

    public event Action EventActivated;
    
    public void Cancel()
    {
        if (_delayCoroutine == null) {
            return;
        }
        StopCoroutine(_delayCoroutine);
        _delayCoroutine = null;
    }

    public void Process()
    {
        Cancel();
        StartCoroutine(WaitAndActivate(_delaySec, _activateAfterDelay));
    }
    
    private IEnumerator WaitAndActivate(float delaySec, bool activateAfterDelay)
    {
        yield return new WaitForSeconds(delaySec);
        _activator.SetActive(activateAfterDelay);
        _delayCoroutine = null;
        
        EventActivated?.Invoke();
    }
    
}