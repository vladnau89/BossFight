
namespace Opsive.UltimateCharacterController.Demo.BehaviorDesigner
{
    using Opsive.Shared.Inventory;
    using Opsive.UltimateCharacterController.Traits;
    using UnityEngine;

    /// <summary>
    /// Helper functions for the demo behavior tree.
    /// </summary>
    public class DemoAgent : MonoBehaviour
    {
        private Health m_Health;
        private Inventory.Inventory m_Inventory;
        private IItemIdentifier m_ItemIdentifier;

        // Expose the health and ammo via a Behavior Designer property mapping.
        public float Health { get { return m_Health.HealthValue; } }
        public int Ammo { get { return m_ItemIdentifier != null ? m_Inventory.GetItemIdentifierAmount(m_ItemIdentifier) : int.MaxValue; } }

        /// <summary>
        /// Initialize the default values.
        /// </summary>
        private void Start()
        {
            m_Health = GetComponent<Health>();
            m_Inventory = GetComponent<Inventory.Inventory>();

            // Find the ItemIdentifier.
            var itemIdentifier = m_Inventory.DefaultLoadout[0].ItemIdentifier;
            var item = m_Inventory.GetItem(itemIdentifier, 0);
            // If the first DefaultLoadout element is an item then the consumable ItemIdentifier should be retrieved.
            if (item != null) {
                var itemActions = item.ItemActions;
                for (int i = 0; i < itemActions.Length; ++i) {
                    var usableItem = itemActions[i] as Items.Actions.IUsableItem;
                    if (usableItem != null) {
                        m_ItemIdentifier = usableItem.GetConsumableItemIdentifier();
                        break;
                    }
                }
            } else {
                m_ItemIdentifier = itemIdentifier;
            }
        }
    }
}