using RevitAction.Action;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace RevitJournalUI.Tasks.Actions.Parameter
{
    public interface IParameterViewModel : INotifyPropertyChanged
    {
        string Name { get; }

        string Value { get; set; }

        ParameterKind Kind { get; }

        bool IsEnable { get; set; }

        Visibility Visibility { get; set; }

        ICommand Command { get; set; }

        bool HasCommand { get; }

        ICommand DefaultCommand { get; }

        bool HasDefaultCommand { get; }

        void OnOtherChanged(object sender, PropertyChangedEventArgs arg);
    }
}
