using System;

namespace RevitAction.Report
{
    public class ActionManager
    {
        public Guid InitialActionId { get; } = new Guid("df82cb97-c5b9-4fa8-a84c-6da99eb05b71");

        public bool IsInitialAction(Guid actionId)
        {
            return InitialActionId.Equals(actionId);
        }

        public Guid OpenActionId { get; } = new Guid("442b4117-76bf-4421-a516-73ecf3b1a397");

        public bool IsOpenAction(Guid actionId)
        {
            return OpenActionId.Equals(actionId);
        }

        public Guid SaveActionId { get; } = new Guid("f3c2f463-09d8-4943-a7a6-7e78f47c0bbf");

        public bool IsSaveAction(Guid actionId)
        {
            return SaveActionId.Equals(actionId);
        }

        public Guid CloseActionId { get; } = new Guid("aee0d8c6-7292-41b3-80ca-b3353162cfba");

        public bool IsCloseAction(Guid actionId)
        {
            return CloseActionId.Equals(actionId);
        }

        public Guid JournalActionId { get; } = new Guid("c336ec5c-f056-4bef-8022-060259a0d819");

        public bool IsJournalAction(Guid actionId)
        {
            return JournalActionId.Equals(actionId);
        }

        public Guid PurgeUnusedId { get;}

        public bool IsPurgeUnusedId(Guid actionId)
        {
            return PurgeUnusedId.Equals(actionId);
        }


        public bool IsAppAction(Guid actionId)
        {
            return IsInitialAction(actionId)
                || IsJournalAction(actionId)
                || IsCloseAction(actionId);
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
