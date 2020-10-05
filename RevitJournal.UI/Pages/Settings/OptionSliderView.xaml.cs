using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace RevitJournalUI.Pages.Settings
{
    /// <summary>
    /// Interaction logic for OptionView.xaml
    /// </summary>
    public partial class OptionSliderView : UserControl
    {
        private OptionSliderViewModel<TValue> GetViewModel<TValue>() where TValue : class
        {
            return DataContext as OptionSliderViewModel<TValue>;
        }

        public OptionSliderView()
        {
            InitializeComponent();
        }
    }
}
