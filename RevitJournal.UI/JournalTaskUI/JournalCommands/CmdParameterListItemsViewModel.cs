using DataSource.Model.SharedParameters;
using RevitJournal.Revit.SharedParameters;
using RevitJournalUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace RevitJournalUI.JournalTaskUI.JournalCommands
{
    public class CmdParameterListItemsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string ParameterValue { get; set; }

        public IEnumerable<SharedParameter> SharedParameters { get; private set; } = new List<SharedParameter>();

        public ObservableCollection<CheckedDisplayViewModel> ParameterValues { get; }
            = new ObservableCollection<CheckedDisplayViewModel>();

        public void UpdateParameterValues(IList<SharedParameter> sharedParameters, IList<string> preSelected)
        {
            if (sharedParameters is null || sharedParameters.Count == 0) { return; }
            if(preSelected is null) { preSelected = new List<string>(); }

            ParameterValues.Clear();
            var comparer = new SharedParameterComparer();
            SharedParameters = sharedParameters.OrderBy(par => par, comparer);
            foreach (var parameter in SharedParameters)
            {
                var parameterName = parameter.Name;
                var isChecked = preSelected.Contains(parameterName);
                var model = new CheckedDisplayViewModel
                {
                    Checked = isChecked,
                    DisplayName = parameterName,
                    GroupName = parameter.Group.Name
                };
                model.PropertyChanged += new PropertyChangedEventHandler(OnCheckedChanged);
                ParameterValues.Add(model);
            }
        }

        private bool _IsOkEnabled = false;
        public bool IsOkEnabled
        {
            get { return _IsOkEnabled; }
            set
            {
                if (_IsOkEnabled == value) { return; }

                _IsOkEnabled = value;
                OnPropertyChanged(nameof(IsOkEnabled));
            }
        }

        private string _SelectedParameters = string.Empty;
        public string SelectedParameters
        {
            get { return _SelectedParameters; }
            set
            {
                if (_SelectedParameters.Equals(value, StringComparison.CurrentCulture)) { return; }

                _SelectedParameters = value;
                OnPropertyChanged(nameof(SelectedParameters));
            }
        }

        private void OnCheckedChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args is null || !(sender is CheckedDisplayViewModel model)
                || args.PropertyName.Equals(nameof(model.Checked), StringComparison.CurrentCulture) == false) { return; }

            IsOkEnabled = CheckedSharedParameters().Count > 0;
            if (SelectedParameters.Contains(model.DisplayName) && model.Checked == false)
            {
                var replaceValue = string.Concat(model.DisplayName, Environment.NewLine);
                SelectedParameters = SelectedParameters.Replace(replaceValue, string.Empty);
            }
            else if (SelectedParameters.Contains(model.DisplayName) == false && model.Checked)
            {
                if (string.IsNullOrWhiteSpace(SelectedParameters))
                {
                    SelectedParameters = string.Concat(SelectedParameters, model.DisplayName);
                }
                else
                {
                    SelectedParameters = string.Concat(SelectedParameters, Environment.NewLine, model.DisplayName);
                }
            }
        }

        public IList<SharedParameter> CheckedSharedParameters()
        {
            var sharedParameters = new List<SharedParameter>();
            var checkedItems = ParameterValues.Where(par => par.Checked);
            foreach (var sharedParameter in SharedParameters)
            {
                foreach (var checkedItem in checkedItems)
                {
                    if (IsSame(sharedParameter, checkedItem) == false
                        || sharedParameters.Contains(sharedParameter)) { continue; }

                    sharedParameters.Add(sharedParameter);
                }
            }
            return sharedParameters;
        }

        private static bool IsSame(SharedParameter parameter, CheckedDisplayViewModel model)
        {
            return parameter != null
                && model != null
                && parameter.Name.Equals(model.DisplayName, StringComparison.CurrentCulture)
                && parameter.Group.Name.Equals(model.GroupName, StringComparison.CurrentCulture);
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
    public class SharedParameterComparer : IComparer<SharedParameter>
    {
        public int Compare(SharedParameter parameter, SharedParameter other)
        {
            if (parameter is null && other is null) { return 0; }
            if (parameter is null && other != null) { return 1; }
            if (parameter != null && other is null) { return -1; }
            if (parameter is null || other is null) { return 0; }

            if (parameter.Group is null && other.Group is null) { return 0; }
            if (parameter.Group is null && other.Group != null) { return 1; }
            if (parameter.Group != null && other.Group is null) { return -1; }

            var compared = string.Compare(parameter.Group.Name, other.Group.Name, StringComparison.CurrentCulture);
            if (compared != 0) { return compared; }

            return string.Compare(parameter.Name, other.Name, StringComparison.CurrentCulture);
        }
    }
}
