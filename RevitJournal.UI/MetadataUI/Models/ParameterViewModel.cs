using DataSource.Model.Family;
using RevitJournal.Duplicate.Comparer;
using System.Reflection;
using Utilities.System;
using Utilities.UI;

namespace RevitJournalUI.MetadataUI.Models
{
    public class ParameterViewModel : ANotifyPropertyChangedModel
    {
        private const string ParameterDistanceSuffix = "Distance";

        public Parameter Parameter { get; set; }

        public string Id
        {
            get { return Parameter.Id; }
            set
            {
                if (StringUtils.Equals(Parameter.Id, value)) { return; }

                Parameter.Id = value;
                NotifyPropertyChanged();
            }
        }

        private string idDistance = string.Empty;
        public string IdDistance
        {
            get { return idDistance; }
            set
            {
                if (StringUtils.Equals(idDistance, value)) { return; }

                idDistance = value;
                NotifyPropertyChanged();
            }
        }


        public string Name
        {
            get { return Parameter.Name; }
            set
            {
                if (StringUtils.Equals(Parameter.Name, value)) { return; }

                Parameter.Name = value;
                NotifyPropertyChanged();
            }
        }

        private string nameDistance = string.Empty;
        public string NameDistance
        {
            get { return nameDistance; }
            set
            {
                if (StringUtils.Equals(nameDistance, value)) { return; }

                nameDistance = value;
                NotifyPropertyChanged();
            }
        }

        public virtual string Value
        {
            get { return Parameter.Value; }
            set
            {
                if (StringUtils.Equals(Parameter.Value, value)) { return; }

                Parameter.Value = value;
                NotifyPropertyChanged();
            }
        }

        private string valueDistance = string.Empty;
        public string ValueDistance
        {
            get { return valueDistance; }
            set
            {
                if (StringUtils.Equals(valueDistance, value)) { return; }

                valueDistance = value;
                NotifyPropertyChanged();
            }
        }

        public string ValueType
        {
            get { return Parameter.ValueType; }
            set
            {
                if (StringUtils.Equals(Parameter.ValueType, value)) { return; }

                Parameter.ValueType = value;
                NotifyPropertyChanged();
            }
        }

        private string valueTypeDistance = string.Empty;
        public string ValueTypeDistance
        {
            get { return valueTypeDistance; }
            set
            {
                if (StringUtils.Equals(valueTypeDistance, value)) { return; }

                valueTypeDistance = value;
                NotifyPropertyChanged();
            }
        }

        public string ParameterType
        {
            get { return Parameter.ParameterType; }
            set
            {
                if (StringUtils.Equals(Parameter.ParameterType, value)) { return; }

                Parameter.ParameterType = value;
                NotifyPropertyChanged();
            }
        }

        private string parameterTypeDistance = string.Empty;
        public string ParameterTypeDistance
        {
            get { return parameterTypeDistance; }
            set
            {
                if (StringUtils.Equals(parameterTypeDistance, value)) { return; }

                parameterTypeDistance = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsInstance
        {
            get { return Parameter.IsInstance; }
            set
            {
                if (Parameter.IsInstance == value) { return; }

                Parameter.IsInstance = value;
                NotifyPropertyChanged();
            }
        }

        private string isInstanceDistance = string.Empty;
        public string IsInstanceDistance
        {
            get { return isInstanceDistance; }
            set
            {
                if (StringUtils.Equals(isInstanceDistance, value)) { return; }

                isInstanceDistance = value;
                NotifyPropertyChanged();
            }
        }

        public void UpdateDistance(Parameter parameter, IModelDuplicateComparer<Parameter> comparer)
        {
            if (parameter is null || comparer is null) { return; }

            foreach (var field in Parameter.GetType().GetProperties())
            {
                var fieldName = field.Name;
                if (HasParameter(fieldName, out var modelParameter) == false) { continue; }

                if (comparer.HasByName(fieldName, out var fieldComparer) == false) { continue; }

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
    }
}
