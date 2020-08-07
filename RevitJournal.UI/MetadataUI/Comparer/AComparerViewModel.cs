using RevitJournal.Duplicate.Comparer;
using RevitJournalUI.Models;
using System;

namespace RevitJournalUI.MetadataUI.Comparer
{
    public abstract class AMetadataComparerViewModel<TModel> : ACheckedViewModel
    {
        public IDuplicateComparer<TModel> Comparer { get; private set; }

        protected AMetadataComparerViewModel(IDuplicateComparer<TModel> comparer)
        {
            if (comparer is null) { throw new ArgumentNullException(nameof(comparer)); }

            Comparer = comparer;
            Checked = comparer.UseComparer;
        }

        public override bool Checked
        {
            get { return Comparer.UseComparer; }
            set
            {
                if (Comparer.UseComparer == value) { return; }

                Comparer.UseComparer = value;
                OnPropertyChanged(nameof(Checked));
            }
        }

        public string PropertyName { get { return Comparer.DisplayName; } }
    }
}
