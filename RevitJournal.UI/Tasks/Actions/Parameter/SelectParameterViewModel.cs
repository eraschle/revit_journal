﻿using RevitAction.Action.Parameter;
using System.Collections.ObjectModel;

namespace RevitJournalUI.Tasks.Actions.Parameter
{
    public class SelectParameterViewModel : ParameterViewModel
    {
        private ActionParameterSelect ParameterSelect
        {
            get { return Parameter as ActionParameterSelect; }
        }

        public SelectParameterViewModel(ActionParameterSelect parameter, bool isEnable = true) : base(parameter, isEnable)
        {
            AddValues();
        }

        public ObservableCollection<string> ParameterValues { get; } = new ObservableCollection<string>();

        private void AddValues()
        {
            if (ParameterSelect.Values is null 
                || ParameterSelect.Values.Count == 0) { return; }

            ParameterValues.Clear();
            foreach (var value in ParameterSelect.Values)
            {
                ParameterValues.Add(value);
            }
        }
    }
}