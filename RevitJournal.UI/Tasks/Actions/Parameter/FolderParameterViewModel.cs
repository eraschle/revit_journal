using RevitAction.Action;
using Utilities;
using Utilities.UI.Helper;

namespace RevitJournalUI.Tasks.Actions.Parameter
{
    public class FolderParameterViewModel : ParameterViewModel
    {
        public string Title { get; set; }

        public FolderParameterViewModel(IActionParameter parameter) : base(parameter)
        {
            Command = new RelayCommand<string>(ChooseAction, ChoosePredicate);
        }

        private bool ChoosePredicate(string parameter)
        {
            return true;
        }

        private void ChooseAction(string parameter)
        {
            if (string.IsNullOrEmpty(parameter))
            {
                parameter = DefaultValue;
            }
            Value = PathDialog.ChooseDir(Title, parameter);
        }

        public override string Value
        {
            get { return base.Value; }
            set
            {
                if (StringUtils.Equals(value, DefaultValue))
                {
                    value = string.Empty;
                }
                base.Value = value;
            }
        }
    }
}
