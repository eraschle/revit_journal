using DataSource.Model.Metadata;
using RevitJournal.Duplicate.Comparer;

namespace RevitJournalUI.MetadataUI.Comparer
{
    public class ParameterComparerViewModel : AMetadataComparerViewModel<Parameter>
    {
        public ParameterComparerViewModel(IDuplicateComparer<Parameter> comparer)
            : base(comparer) { }
    }
}
