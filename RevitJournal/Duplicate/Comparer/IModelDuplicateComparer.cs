using System.Collections.Generic;

namespace RevitJournal.Duplicate.Comparer
{
    public interface IModelDuplicateComparer<TModel>
    {
        ICollection<IDuplicateComparer<TModel>> PropertyComparers { get; set; }

        IDuplicateComparer<TModel> ByName(string propertyName);

        bool HasByName(string propertyName, out IDuplicateComparer<TModel> comparer);

    }
}
