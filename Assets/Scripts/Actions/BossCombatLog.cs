using BehaviorDesigner.Runtime;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.UltimateCharacterController
{
    [TaskDescription("Logs text when combat debug log is enabled (Boss Combat Settings → Enable Behavior Tree Log).")]
    [TaskCategory("Ultimate Character Controller")]
    public class BossCombatLog : Action
    {
        public SharedString text;
        public SharedBool logError;
        public SharedBool logTime;

        public override TaskStatus OnUpdate()
        {
            if (!BossCombatLogUtility.IsLogEnabled(Owner)) {
                return TaskStatus.Success;
            }

            var message = text != null ? text.Value : string.Empty;
            var includeTime = logTime != null && logTime.Value;
            var context = Owner != null ? Owner.gameObject : null;

            if (includeTime) {
                message = $"{Time.time:F2}: {message}";
            }

            if (logError != null && logError.Value) {
                Debug.LogError(message, context);
            } else {
                Debug.Log(message, context);
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
