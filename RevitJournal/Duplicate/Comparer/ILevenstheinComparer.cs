using System.Collections.Generic;

namespace RevitJournal.Duplicate.Comparer
{
    public interface ILevenstheinComparer<TModel> : IEqualityComparer<TModel>, IComparer<TModel>
    {
        int LevenstheinDistance(TModel model, TModel other);

        string LevenstheinDistanceAsString(TModel model, TModel other);
    }
}
