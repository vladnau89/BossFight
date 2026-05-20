using Opsive.Shared.Events;
using Opsive.UltimateCharacterController.Traits;
using UnityEngine;
using UnityEngine.UI;

namespace BossFight.UI
{
    /// <summary>
    /// Updates a scene-authored boss health slider (bottom of screen).
    /// </summary>
    [DisallowMultipleComponent]
    public class BossHealthBarUI : MonoBehaviour
    {
        [SerializeField] private GameObject _boss;
        [SerializeField] private Slider _slider;
        [SerializeField] private string _healthAttributeName = "Health";

        private Attribute _healthAttribute;

        private void Start()
        {
            if (_slider == null) {
                Debug.LogWarning("BossHealthBarUI: Slider is not assigned.", this);
                gameObject.SetActive(false);
                return;
            }

            if (!BindBoss()) {
                gameObject.SetActive(false);
                return;
            }

            RegisterEvents();
            Refresh();
        }

        private void OnEnable()
        {
            if (_boss != null && _healthAttribute != null) {
                RegisterEvents();
            }
        }

        private void OnDisable()
        {
            UnregisterEvents();
        }

        private void RegisterEvents()
        {
            if (_boss == null) {
                return;
            }

            EventHandler.RegisterEvent<Attribute>(_boss, "OnAttributeUpdateValue", OnAttributeUpdateValue);
            EventHandler.RegisterEvent<Vector3, Vector3, GameObject>(_boss, "OnDeath", OnBossDeath);
        }

        private void UnregisterEvents()
        {
            if (_boss == null) {
                return;
            }

            EventHandler.UnregisterEvent<Attribute>(_boss, "OnAttributeUpdateValue", OnAttributeUpdateValue);
            EventHandler.UnregisterEvent<Vector3, Vector3, GameObject>(_boss, "OnDeath", OnBossDeath);
        }

        private bool BindBoss()
        {
            if (_boss == null) {
                var combat = FindObjectOfType<BossCombat>();
                if (combat != null) {
                    _boss = combat.gameObject;
                }
            }

            if (_boss == null) {
                Debug.LogWarning("BossHealthBarUI: boss not found.", this);
                return false;
            }

            var attributeManager = _boss.GetComponent<AttributeManager>();
            if (attributeManager == null) {
                Debug.LogWarning("BossHealthBarUI: AttributeManager missing on boss.", this);
                return false;
            }

            _healthAttribute = attributeManager.GetAttribute(_healthAttributeName);
            if (_healthAttribute == null) {
                Debug.LogWarning($"BossHealthBarUI: attribute '{_healthAttributeName}' not found.", this);
                return false;
            }

            return true;
        }

        private void OnAttributeUpdateValue(Attribute attribute)
        {
            if (attribute != _healthAttribute) {
                return;
            }

            Refresh();
        }

        private void OnBossDeath(Vector3 position, Vector3 force, GameObject attacker)
        {
            if (_slider != null) {
                _slider.value = 0f;
            }
        }

        private void Refresh()
        {
            if (_slider == null || _healthAttribute == null) {
                return;
            }

            var range = _healthAttribute.MaxValue - _healthAttribute.MinValue;
            _slider.value = range > 0f
                ? (_healthAttribute.Value - _healthAttribute.MinValue) / range
                : 0f;
        }
    }
}
