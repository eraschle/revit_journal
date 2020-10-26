using DataSource.Models.FileSystem;
using RevitJournal.Library.Filtering;

namespace RevitJournalUI.Pages.Files.Filter
{
    public class FilterViewModel
    {
        public string Group
        {
            get { return Rule.Name; }
        }

        public IFilterRule<RevitFamilyFile> Rule { get; private set; }

        public string Filter
        {
            get { return Value.Name; }
        }

        public FilterValue Value { get; private set; }

        public FilterViewModel(IFilterRule<RevitFamilyFile> rule, FilterValue value)
        {
            Rule = rule;
            Value = value;
        }
    }
}
