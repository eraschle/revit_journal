using RevitJournal.Journal.Command;
using RevitJournal.Revit.Commands.Parameter;
using RevitJournalUI.Helper;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RevitJournalUI.JournalTaskUI.JournalCommands
{
    public class CmdParameterListViewModel : CmdParameterStringViewModel
    {
        public CmdParameterListViewModel() : base()
        {
            ChooseParametersCommand = new RelayCommand<object>(ChooseParametersCommandAction);
        }

        public ObservableCollection<string> ParameterValues { get; }
            = new ObservableCollection<string>();

        public ICommand ChooseParametersCommand { get; }

        public void ChooseParametersCommandAction(object parameter)
        {
            if (!(CommandParameter is SharedParameterCommandParameter model)) { return; }

            var preSelected = ParameterListConverter.GetList(ParameterValue);
            var dialog = new CmdParameterListItemsView(model.ParameterValues, preSelected);
            if (dialog.ShowDialog() == false) { return; }

            var sharedNames = dialog.SelectedParameterNames;
            var parameterValue = string.Empty;
            if (sharedNames.Count > 0)
            {
                parameterValue = ParameterListConverter.GetLine(sharedNames);
            }
            ParameterValue = parameterValue;
        }
    }
}
