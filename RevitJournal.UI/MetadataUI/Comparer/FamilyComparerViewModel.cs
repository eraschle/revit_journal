﻿using DataSource.Model.Family;
using RevitJournal.Duplicate.Comparer;

namespace RevitJournalUI.MetadataUI.Comparer
{
    public class FamilyComparerViewModel : AMetadataComparerViewModel<Family>
    {
        public FamilyComparerViewModel(IDuplicateComparer<Family> comparer)
            : base(comparer) { }
    }
}
