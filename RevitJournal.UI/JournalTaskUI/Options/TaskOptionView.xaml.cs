using System;
using System.Windows.Controls;
using System.Windows.Data;
using Utilities.System;

namespace RevitJournalUI.JournalTaskUI.Options
{
    /// <summary>
    /// Interaction logic for JournalTaskOptionUserControl.xaml
    /// </summary>
    public partial class TaskOptionView : UserControl
    {
        private const string prefixTimeoutTitle = "Timeout";

        private TaskOptionViewModel ViewModel
        {
            get { return DataContext as TaskOptionViewModel; }
        }

        public TaskOptionView()
        {
            InitializeComponent();
        }

        private void OnTimeoutUpdated(object sender, DataTransferEventArgs args)
        {
            if(!(sender is Label label)){ return; }

            var timeout = DateUtils.AsString(ViewModel.Options.Timeout, format: DateUtils.Minute);
            var timeoutTitle = string.Concat(prefixTimeoutTitle, " [", timeout, " min]");
            label.Content = timeoutTitle;
            args.Handled = true;
        }
    }
}
