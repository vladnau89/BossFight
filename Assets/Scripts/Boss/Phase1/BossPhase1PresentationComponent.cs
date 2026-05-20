using UnityEngine;

[DisallowMultipleComponent]
public sealed class BossPhase1PresentationComponent : MonoBehaviour
{
    [SerializeField] private GameObject _giantHandRoot;
    [SerializeField] private GameObject _rangedWeaponRoot;

    public GameObject GiantHandRoot => _giantHandRoot;

    private void Awake()
    {
        _giantHandRoot.SetActive(false);
    }

    public void ShowRanged()
    {
        _giantHandRoot.SetActive(false);
        _rangedWeaponRoot.SetActive(true);
    }

    public void ShowGiantHandPrep()
    {
        _rangedWeaponRoot.SetActive(false);
        _giantHandRoot.SetActive(false);
    }

    public void HideGiantHand()
    {
        _giantHandRoot.SetActive(false);
    }
}
