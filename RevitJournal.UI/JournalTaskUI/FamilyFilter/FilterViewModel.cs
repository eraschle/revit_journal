using DataSource.Model.FileSystem;
using RevitJournal.Library.Filtering;

namespace RevitJournalUI.JournalTaskUI.FamilyFilter
{
    public class FilterViewModel
    {
        public string Group
        {
            get { return Rule.Name; }
        }

        public IFilterRule<RevitFamily> Rule { get; private set; }

        public string Filter
        {
            get { return Value.Name; }
        }

        public FilterValue Value { get; private set; }

        public FilterViewModel(IFilterRule<RevitFamily> rule, FilterValue value)
        {
            Rule = rule;
            Value = value;
        }
    }
}
