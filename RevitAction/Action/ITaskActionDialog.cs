using System.Collections.Generic;

namespace RevitAction.Action
{
    public interface ITaskActionDialog : ITaskActionCommand
    {
        string DialogId { get; }

        IEnumerable< string> AllowedDialogs { get; }

        string DialogButton { get; }
    }
}
