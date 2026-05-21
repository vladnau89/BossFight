using BehaviorDesigner.Runtime;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.UltimateCharacterController
{
    [TaskDescription("Logs text when EnableLog is true on the behavior tree (set from Boss Combat Settings).")]
    [TaskCategory("Ultimate Character Controller")]
    public class BossCombatLog : Action
    {
        public SharedString text;
        public SharedBool logError;
        public SharedBool logTime;

        public override TaskStatus OnUpdate()
        {
            if (!BossCombatLogUtility.IsEnabled(Owner)) {
                return TaskStatus.Success;
            }

            var message = text != null ? text.Value : string.Empty;
            if (logTime != null && logTime.Value) {
                message = $"{Time.time:F2}: {message}";
            }

            if (logError != null && logError.Value) {
                Debug.LogError(message, Owner != null ? Owner.gameObject : null);
            } else {
                Debug.Log(message, Owner != null ? Owner.gameObject : null);
            }

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            text = string.Empty;
            logError = false;
            logTime = false;
        }
    }
}
