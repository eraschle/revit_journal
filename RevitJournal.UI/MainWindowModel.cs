using System.Windows;
using Utilities.UI;

namespace RevitJournalUI
{
    public class MainWindowModel : ANotifyPropertyChangedModel
    {
        public MainWindowModel()
        {
        }

        public StatusBarViewModel StatusBar
        {
            get { return StatusBarViewModel.Instance; }
        }
    }
}
