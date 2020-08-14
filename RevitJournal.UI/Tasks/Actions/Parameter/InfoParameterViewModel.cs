using RevitAction.Action;

namespace RevitJournalUI.Tasks.Actions.Parameter
{
    public class InfoParameterViewModel : ParameterViewModel
    {
        public InfoParameterViewModel(IActionParameter parameter, bool isEnable = true)
            : base(parameter, isEnable) { }
    }
}
