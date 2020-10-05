using System;
using System.Windows;
using System.Windows.Input;

namespace RevitJournalUI.Commands
{
    public class DefaultValueCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly DependencyProperty property;
        private readonly object value;

        public DefaultValueCommand(DependencyProperty dependencyProperty, object defaultValue)
        {
            property = dependencyProperty;
            value = defaultValue;
        }

        public bool CanExecute(object parameter)
        {
            return parameter is FrameworkElement;
        }

        public void Execute(object parameter)
        {
            var element = parameter as FrameworkElement;
            if(element is null) { return; }

            var bindingExpression = element.GetBindingExpression(property);
            if (bindingExpression is null) { return; }

            var propertyName = bindingExpression.ParentBinding.Path.Path;
            var propertyInfo = bindingExpression.DataItem.GetType().GetProperty(propertyName);
            if (propertyInfo is null) { return; }

            propertyInfo.SetValue(bindingExpression.DataItem, value, null);
        }
    }
}
