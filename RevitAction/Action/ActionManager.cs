using RevitAction.Action;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;

namespace RevitAction.Report
{
    public class ActionManager
    {
        public const string CustomStartMessage = "Custom_Action_Started";
        public const string CustomFinishMessage = "Custom_Action_Finished";

        public const string InitialMessage = "Initial_Action_Message";
        public const string InitialActionName = "Initial Action";

        public static bool IsCustomStart(string message)
        {
            return StringUtils.Equals(CustomStartMessage, message);
        }

        public static bool IsCustomFinish(string message)
        {
            return StringUtils.Equals(CustomFinishMessage, message);
        }

        public static Guid InitialActionId { get; } = new Guid("df82cb97-c5b9-4fa8-a84c-6da99eb05b71");

        public static bool IsInitialAction(Guid actionId)
        {
            return InitialActionId.Equals(actionId);
        }

        public static Guid OpenActionId { get; } = new Guid("442b4117-76bf-4421-a516-73ecf3b1a397");

        public static bool IsOpenAction(Guid actionId)
        {
            return OpenActionId.Equals(actionId);
        }

        public static Guid SaveActionId { get; } = new Guid("f3c2f463-09d8-4943-a7a6-7e78f47c0bbf");

        public static bool IsSaveAction(Guid actionId)
        {
            return SaveActionId.Equals(actionId);
        }

        public static Guid JournalActionId { get; } = new Guid("c336ec5c-f056-4bef-8022-060259a0d819");

        public static bool IsJournalAction(Guid actionId)
        {
            return JournalActionId.Equals(actionId);
        }

        public bool IsDefaultAction(Guid actionId)
        {
            return IsInitialAction(actionId) 
                || IsJournalAction(actionId) 
                || IsOpenAction(actionId) 
                || IsSaveAction(actionId);
        }

        public bool IsCostumnAction(Guid actionId)
        {
            return IsDefaultAction(actionId) == false
                && TaskActions.Any(action => action.ActionId.Equals(actionId));
        }

        public bool HasDialogAction(Guid actionId, out TaskDialogAction dialogAction)
        {
            dialogAction = TaskActions.FirstOrDefault(act => act.ActionId.Equals(actionId));
            return dialogAction is object;
        }

        public string GetActionName(Guid actionId)
        {
            if (HasDialogAction(actionId, out var dialogAction))
            {
                return dialogAction.Name;
            }
            var actionName = InitialActionName;
            if (IsJournalAction(actionId))
            {
                actionName = "Journal";
            }
            return actionName;
        }

        public List<TaskDialogAction> TaskActions { get; } = new List<TaskDialogAction>();

        public void AddTaskActions(IEnumerable<TaskDialogAction> taskActions)
        {
            TaskActions.Clear();
            TaskActions.AddRange(taskActions);
        }

        public bool IsAllowedDialogs(string dialogId, out DialogHandler dialogHandler)
        {
            dialogHandler = null;
            foreach (var taskAction in TaskActions)
            {
                if (taskAction.HasDialogHandler(dialogId, out var handler) == false) { continue; }

                handler.ActionId = taskAction.ActionId;
                dialogHandler = handler;
                break;
            }
            return dialogHandler is object;
        }


        public Guid GetNextAction(Guid actionId)
        {
            if (IsInitialAction(actionId))
            {
                return OpenActionId;
            }
            if (IsOpenAction(actionId))
            {
                return JournalActionId;
            }
            if (IsJournalAction(actionId))
            {
                actionId = OpenActionId;
            }
            return HasNextAction(actionId, out var nextAction)
                ? nextAction.ActionId
                : TaskActions.Last().ActionId;
        }


        public bool HasNextAction(Guid actionId, out TaskDialogAction nextAction)
        {
            nextAction = null;
            if (TaskActions.Any(act => act.ActionId.Equals(actionId)))
            {
                var index = TaskActions.FindIndex(act => act.ActionId.Equals(actionId)) + 1;
                nextAction = index < TaskActions.Count ? TaskActions[index] : null;
            }
            return nextAction != null;
        }
    }
}
