using System;

namespace RevitAction.Action.Revit
{
    public abstract class ARevitTaskAction<TAction> : IRevitTaskAction where TAction : ITaskAction
    {
        public TAction Action { get; private set; }

        protected ARevitTaskAction(TAction action)
        {
            Action = action;
        }

        public void Execute(RevitActionData actionData)
        {
            try
            {
                ExecuteAction(actionData);
                actionData.Success.Add(Action);
            }
            catch (Exception ex)
            {
                actionData.Error.Add(ex);
                throw;
            }
        }

        protected abstract void ExecuteAction(RevitActionData actionData);
    }
}