using RevitAction.Action;
using System.IO;
using Utilities.System;
using Utilities.UI.Helper;

namespace RevitJournalUI.Tasks.Actions.Parameter
{
    public class FileParameterViewModel : ParameterViewModel
    {
        public string Title { get; set; }

        public string Filter { get; set; }

        public string Pattern { get; set; }

        public SearchOption Options { get; set; } = SearchOption.AllDirectories;

        public FileParameterViewModel(IActionParameter parameter) : base(parameter)
        {
            Command = new RelayCommand<string>(ChooseAction, ChoosePredicate);
        }

        private bool ChoosePredicate(string parameter)
        {
            return true;
        }

        private void ChooseAction(string parameter)
        {
            parameter = DirUtils.GetDirectory(parameter);
            Value = FileUtils.GetFirstFile(parameter, Pattern);
            Value = PathDialog.OpenFile(Title, Filter, Pattern, parameter);
        }
    }

}
