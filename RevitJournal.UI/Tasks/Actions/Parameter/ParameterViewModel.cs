using RevitAction.Action;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Utilities;
using Utilities.UI.Helper;

namespace RevitJournalUI.Tasks.Actions.Parameter
{
    public class ParameterViewModel : IParameterViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public IActionParameter Parameter { get; }

        protected string DefaultValue { get; set; }

        public ParameterViewModel(IActionParameter parameter, bool isEnable = true)
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            Parameter = parameter;
            Value = parameter.Value;
            IsEnable = isEnable;
            SetDefaultCommand(parameter);
            SetClearCommand();
        }

        public string Name { get { return Parameter.Name; } }

        public virtual string Value
        {
            get { return Parameter.Value; }
            set
            {
                if (StringUtils.Equals(Parameter.Value, value)) { return; }

                Parameter.Value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        private bool isEnable;
        public virtual bool IsEnable
        {
            get { return isEnable; }
            set
            {
                if (isEnable == value) { return; }

                isEnable = value;
                OnPropertyChanged(nameof(IsEnable));
            }
        }

        private Visibility visibility = Visibility.Visible;
        public virtual Visibility Visibility
        {
            get { return visibility; }
            set
            {
                if (visibility == value) { return; }

                visibility = value;
                OnPropertyChanged(nameof(Visibility));
            }
        }

        public virtual ICommand Command { get; set; } = null;

        public bool HasCommand { get { return Command != null; } }


        #region Clear Command

        public virtual ICommand ClearCommand { get; set; } = null;

        public bool HasClearCommand { get { return ClearCommand != null; } }

        protected void SetClearCommand()
        {
            ClearCommand = new RelayCommand<string>(ClearCommandAction);
        }

        protected virtual void ClearCommandAction(object parameter)
        {
            Value = string.Empty;
        }

        public virtual Visibility ClearVisibility
        {
            get { return HasClearCommand ? Visibility.Visible : Visibility.Hidden; }
        }

        #endregion

        #region DefaultCommand

        public virtual ICommand DefaultCommand { get; set; } = null;

        public bool HasDefaultCommand { get { return DefaultCommand != null; } }

        protected void SetDefaultCommand(IActionParameter parameter)
        {
            if (parameter is null || string.IsNullOrWhiteSpace(parameter.DefaultValue)) { return; }

            DefaultValue = parameter.DefaultValue;
            DefaultCommand = new RelayCommand<string>(DefaultCommandAction);
        }

        protected virtual void DefaultCommandAction(object parameter)
        {
            Value = DefaultValue;
        }

        public virtual Visibility DefaultVisibility
        {
            get { return HasDefaultCommand ? Visibility.Visible : Visibility.Hidden; }
        }

        #endregion

        public ParameterKind Kind
        {
            get { return Parameter.Kind; }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public virtual void OnOtherChanged(object sender, PropertyChangedEventArgs arg)
        {
            if (arg is null || StringUtils.Equals(arg.PropertyName, nameof(Value)) == false) { return; }

            Value = string.Empty;
        }
    }
}
