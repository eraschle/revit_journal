using RevitAction.Action;
using System.Collections.Generic;

namespace RevitAction.Report
{
    public abstract class AReport
    {
        protected Dictionary<ITaskAction, IList<string>> ActionMessage 
            = new Dictionary<ITaskAction, IList<string>>();

        protected ITaskAction Current { get; private set; }

        public void SetAction(ITaskAction action)
        {
            if(action is null) { return; }

            Current = action;
        }

        public void Add(string message)
        {
            if(Current is null) { return; }

            if(ActionMessage.ContainsKey(Current) == false)
            {
                ActionMessage.Add(Current, new List<string>());
            }

            ActionMessage[Current].Add(message);
        }
    }
}
