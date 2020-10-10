﻿using RevitJournal.Tasks.Options.Parameter;

namespace RevitJournalUI.Pages.Settings.Models
{
    public class OptionBoolViewModel : AOptionViewModel<TaskOption<bool>, bool>
    {
        public OptionBoolViewModel(string name, TaskOption<bool> taskOption, bool showDefaultAtStart)
            : base(name, taskOption, showDefaultAtStart) { }
    }
}