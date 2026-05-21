using UnityEngine;

[DisallowMultipleComponent]
public sealed class GiantHandSlamPresentationComponent : MonoBehaviour
{
    [SerializeField] private GameObject _giantHandRoot;
    [SerializeField] private GameObject _rangedWeaponRoot;

    private void Awake()
    {
        HideGiantHand();
    }

    public void ShowRanged()
    {
        HideGiantHand();
        _rangedWeaponRoot.SetActive(true);
    }

    public void ShowGiantHandPrep()
    {
        _rangedWeaponRoot.SetActive(false);
        HideGiantHand();
    }

    public void HideGiantHand() => _giantHandRoot.SetActive(false);

    public void ShowGiantHand() => _giantHandRoot.SetActive(true);
}
