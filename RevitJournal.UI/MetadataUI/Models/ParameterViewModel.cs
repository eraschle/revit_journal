using DataSource.Model.Family;
using RevitJournal.Duplicate.Comparer;
using System;
using System.ComponentModel;
using System.Reflection;

namespace RevitJournalUI.MetadataUI.Models
{
    public class ParameterViewModel : INotifyPropertyChanged
    {
        private const string ParameterDistanceSuffix = "Distance";

        public event PropertyChangedEventHandler PropertyChanged;

        public Parameter Parameter { get; set; }

        public string Id
        {
            get { return Parameter.Id; }
            set
            {
                if (Parameter.Id.Equals(value, StringComparison.CurrentCulture)) { return; }

                Parameter.Id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        private string _IdDistance = string.Empty;
        public string IdDistance
        {
            get { return _IdDistance; }
            set
            {
                if (_IdDistance.Equals(value, StringComparison.CurrentCulture)) { return; }

                _IdDistance = value;
                OnPropertyChanged(nameof(IdDistance));
            }
        }


        public string Name
        {
            get { return Parameter.Name; }
            set {
                if (Parameter.Name.Equals(value, StringComparison.CurrentCulture)) { return; }

                Parameter.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private string _NameDistance = string.Empty;
        public string NameDistance
        {
            get { return _NameDistance; }
            set
            {
                if (_NameDistance.Equals(value, StringComparison.CurrentCulture)) { return; }

                _NameDistance = value;
                OnPropertyChanged(nameof(NameDistance));
            }
        }

        public virtual string Value
        {
            get { return Parameter.Value; }
            set
            {
                if (Parameter.Value.Equals(value, StringComparison.CurrentCulture)) { return; }

                Parameter.Value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        private string _ValueDistance = string.Empty;
        public string ValueDistance
        {
            get { return _ValueDistance; }
            set
            {
                if (_ValueDistance.Equals(value, StringComparison.CurrentCulture)) { return; }

                _ValueDistance = value;
                OnPropertyChanged(nameof(ValueDistance));
            }
        }

        public string ValueType
        {
            get { return Parameter.ValueType; }
            set
            {
                if (Parameter.ValueType.Equals(value, StringComparison.CurrentCulture)) { return; }

                Parameter.ValueType = value;
                OnPropertyChanged(nameof(ValueType));
            }
        }

        private string _ValueTypeDistance = string.Empty;
        public string ValueTypeDistance
        {
            get { return _ValueTypeDistance; }
            set
            {
                if (_ValueTypeDistance.Equals(value, StringComparison.CurrentCulture)) { return; }

                _ValueTypeDistance = value;
                OnPropertyChanged(nameof(ValueTypeDistance));
            }
        }

        public string ParameterType
        {
            get { return Parameter.ParameterType; }
            set
            {
                if (Parameter.ParameterType.Equals(value, StringComparison.CurrentCulture)) { return; }

                Parameter.ParameterType = value;
                OnPropertyChanged(nameof(ParameterType));
            }
        }

        private string _ParameterTypeDistance = string.Empty;
        public string ParameterTypeDistance
        {
            get { return _ParameterTypeDistance; }
            set
            {
                if (_ParameterTypeDistance.Equals(value, StringComparison.CurrentCulture)) { return; }

                _ParameterTypeDistance = value;
                OnPropertyChanged(nameof(ParameterTypeDistance));
            }
        }

        public bool IsInstance
        {
            get { return Parameter.IsInstance; }
            set
            {
                if (Parameter.IsInstance == value) { return; }

                Parameter.IsInstance = value;
                OnPropertyChanged(nameof(IsInstance));
            }
        }

        private string _IsInstanceDistance = string.Empty;
        public string IsInstanceDistance
        {
            get { return _IsInstanceDistance; }
            set
            {
                if (_IsInstanceDistance.Equals(value, StringComparison.CurrentCulture)) { return; }

                _IsInstanceDistance = value;
                OnPropertyChanged(nameof(IsInstanceDistance));
            }
        }

        public void UpdateDistance(Parameter parameter, IModelDuplicateComparer<Parameter> comparer)
        {
            if(parameter is null || comparer is null) { return; }

            foreach (var field in Parameter.GetType().GetProperties())
            {
                var fieldName = field.Name;
                if(HasParameter(fieldName, out var modelParameter) == false) { continue; }

                if(comparer.HasByName(fieldName, out var fieldComparer) == false) { continue; }

                var distance = fieldComparer.LevenstheinDistanceAsString(Parameter, parameter);
                modelParameter.SetValue(this, distance);
            }
        }

        private bool HasParameter(string name, out PropertyInfo field)
        {
            var propertyName = string.Concat(name, ParameterDistanceSuffix);
            field = GetType().GetProperty(propertyName);
            return field != null;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
