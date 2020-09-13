using System;
using System.Collections.Generic;

namespace RevitAction.Report
{
    public class ActionManager
    {
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

        public bool IsAppAction(Guid actionId)
        {
            return IsInitialAction(actionId)
                || IsJournalAction(actionId);
        }

        public bool IsCustomAction(Guid actionId)
        {
            return IsAppAction(actionId) == false
                && IsDefaultAction(actionId) == false;
        }

        public bool IsDefaultAction(Guid actionId)
        {
            return IsOpenAction(actionId) 
                || IsSaveAction(actionId);
        }

        public IList<string> AllowedDialogs
        {
            get
            {
                return new List<string>
                {
                    "TaskDialog_Replace_Existing_File",
                    "TaskDialog_Newer_File_Exists"
                };
            }
        }

        public bool IsActionId(string actionId, out Guid action)
        {
            if (Guid.TryParse(actionId, out action) == false)
            {
                action = Guid.Empty;
            }
            return action.Equals(Guid.Empty) == false;
        }
    }
}
