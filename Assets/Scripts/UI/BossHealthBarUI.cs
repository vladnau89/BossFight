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
        [SerializeField] private GameObject m_Boss;
        [SerializeField] private Slider m_Slider;
        [SerializeField] private string m_HealthAttributeName = "Health";

        private Attribute m_HealthAttribute;

        private void Start()
        {
            if (m_Slider == null) {
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
            if (m_Boss != null && m_HealthAttribute != null) {
                RegisterEvents();
            }
        }

        private void OnDisable()
        {
            UnregisterEvents();
        }

        private void RegisterEvents()
        {
            if (m_Boss == null) {
                return;
            }

            EventHandler.RegisterEvent<Attribute>(m_Boss, "OnAttributeUpdateValue", OnAttributeUpdateValue);
            EventHandler.RegisterEvent<Vector3, Vector3, GameObject>(m_Boss, "OnDeath", OnBossDeath);
        }

        private void UnregisterEvents()
        {
            if (m_Boss == null) {
                return;
            }

            EventHandler.UnregisterEvent<Attribute>(m_Boss, "OnAttributeUpdateValue", OnAttributeUpdateValue);
            EventHandler.UnregisterEvent<Vector3, Vector3, GameObject>(m_Boss, "OnDeath", OnBossDeath);
        }

        private bool BindBoss()
        {
            if (m_Boss == null) {
                var combat = FindObjectOfType<BossCombat>();
                if (combat != null) {
                    m_Boss = combat.gameObject;
                }
            }

            if (m_Boss == null) {
                Debug.LogWarning("BossHealthBarUI: boss not found.", this);
                return false;
            }

            var attributeManager = m_Boss.GetComponent<AttributeManager>();
            if (attributeManager == null) {
                Debug.LogWarning("BossHealthBarUI: AttributeManager missing on boss.", this);
                return false;
            }

            m_HealthAttribute = attributeManager.GetAttribute(m_HealthAttributeName);
            if (m_HealthAttribute == null) {
                Debug.LogWarning($"BossHealthBarUI: attribute '{m_HealthAttributeName}' not found.", this);
                return false;
            }

            return true;
        }

        private void OnAttributeUpdateValue(Attribute attribute)
        {
            if (attribute != m_HealthAttribute) {
                return;
            }

            Refresh();
        }

        private void OnBossDeath(Vector3 position, Vector3 force, GameObject attacker)
        {
            if (m_Slider != null) {
                m_Slider.value = 0f;
            }
        }

        private void Refresh()
        {
            if (m_Slider == null || m_HealthAttribute == null) {
                return;
            }

            var range = m_HealthAttribute.MaxValue - m_HealthAttribute.MinValue;
            m_Slider.value = range > 0f
                ? (m_HealthAttribute.Value - m_HealthAttribute.MinValue) / range
                : 0f;
        }
    }
}
