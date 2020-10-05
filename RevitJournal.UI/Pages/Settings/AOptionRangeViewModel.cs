using RevitJournal.Tasks.Options.Parameter;

namespace RevitJournalUI.Pages.Settings
{
    public abstract class AOptionRangeViewModel<TOption, TValue> : AOptionViewModel<TOption, TValue> where TOption : TaskOptionRange<TValue>
    {
        protected AOptionRangeViewModel(string name, TOption taskOption) : base(name, taskOption) { }

        public object MinValue
        {
            get { return Option.MinValue; }
        }

        public TValue MaxValue
        {
            get { return Option.MaxValue; }
        }
    }
}
