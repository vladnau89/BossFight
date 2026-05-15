
namespace BehaviorDesigner.Editor.UltimateCharacterController
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.UltimateCharacterController;
    using Opsive.UltimateCharacterController.Character;
    using Opsive.UltimateCharacterController.Editor.Managers;
    using Opsive.UltimateCharacterController.Utility.Builders;
    using UnityEditor;
    using UnityEngine;

    using InspectorUtility = Opsive.UltimateCharacterController.Editor.Inspectors.Utility.InspectorUtility;

    /// <summary>
    /// Draws an inspector that can be used within the Inspector Manager.
    /// </summary>
    [OrderedEditorItem("Behavior Designer", 0)]
    public class InventorySystemIntegration : IntegrationInspector
    {
        private GameObject m_Character;

        /// <summary>
        /// Draws the integration inspector.
        /// </summary>
        public override void DrawInspector()
        {
            ManagerUtility.DrawControlBox("Agent Setup", DrawAgentSetup, "Sets up the character to be used with Behavior Designer.",
                                    IsValidCharacter(), "Setup Agent", SetupAgent, string.Empty);
        }

        /// <summary>
        /// Draws the agent setup fields.
        /// </summary>
        private void DrawAgentSetup()
        {
            m_Character = EditorGUILayout.ObjectField("Character", m_Character, typeof(GameObject), true) as GameObject;
        }

        /// <summary>
        /// Is the Character GameObject a valid character?
        /// </summary>
        private bool IsValidCharacter()
        {
            if (m_Character == null) {
                return false;
            }

            if (m_Character.GetComponent<UltimateCharacterLocomotion>() == null) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Sets up the agent.
        /// </summary>
        private void SetupAgent()
        {
            CharacterBuilder.AddAIAgent(m_Character);
            Opsive.Shared.Editor.Inspectors.Utility.InspectorUtility.AddComponent<BehaviorTreeAgent>(m_Character);
            Opsive.Shared.Editor.Inspectors.Utility.InspectorUtility.AddComponent<BehaviorTree>(m_Character);
        }
    }
}