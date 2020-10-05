using System.Windows.Controls;
using System.Windows;

namespace RevitJournalUI.Tasks
{
    /// <summary>
    /// Interaction logic for JournalTaskListBoxItem.xaml
    /// </summary>
    public partial class TaskView : UserControl
    {
        private TaskViewModel ViewModel
        {
            get { return DataContext as TaskViewModel; }
        }

        public TaskView()
        {
            InitializeComponent();
        }

        //private void UserControl_Loaded(object sender, RoutedEventArgs args)
        //{
        //    if (ViewModel is null) { return; }

        //    lblExecuted.Content = GetExecutedCount();
        //}

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ViewModel is null) { return; }

            //lblExecuted.Content = GetExecutedCount();
            if (ViewModel.HasErrorAction(out var action))
            {
                btnError.Content = action.Name;
            }
        }

        private string GetExecutedCount()
        {
            var executed = 0.0;
            var count = 0.0;
            if (ViewModel.ActionsCount > 0)
            {
                if (ViewModel.ExecutedActions > 0)
                {
                    executed = ViewModel.ExecutedActions;
                }
                count = ViewModel.ActionsCount;
            }
            return $"{executed} / {count}";
        }
    }
}
