using DataSource.Model.Family;
using RevitJournal.Duplicate.Comparer;

namespace RevitJournalUI.MetadataUI.Comparer
{
    public class FamilyTypeComparerViewModel : AMetadataComparerViewModel<FamilyType>
    {
        public FamilyTypeComparerViewModel(IDuplicateComparer<FamilyType> comparer)
            : base(comparer) { }
    }
}
