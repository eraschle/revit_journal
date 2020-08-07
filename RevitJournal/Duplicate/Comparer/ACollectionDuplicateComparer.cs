using System;
using System.Collections.Generic;

namespace RevitJournal.Duplicate.Comparer
{
    public abstract class ACollectionDuplicateComparer<TModel, TProperty> : IDuplicateComparer<TModel>
    {
        public ILevenstheinComparer<TProperty> ItemComparer { get; set; }

        public bool UseComparer { get; set; }

        public string PropertyName { get; private set; }

        public string DisplayName { get; private set; }

        protected ACollectionDuplicateComparer(bool dublicateComparer, string propertyName, string displayName, ILevenstheinComparer<TProperty> comparer)
        {
            UseComparer = dublicateComparer;
            ItemComparer = comparer;
            PropertyName = propertyName;
            DisplayName = displayName;
        }

        public bool Equals(TModel collection, TModel other)
        {
            var equals = true;
            var bigger = GetBigger(GetProperty(collection), GetProperty(other));
            foreach (var model in GetSmaller(GetProperty(collection), GetProperty(other)))
            {
                equals &= bigger.Contains(model);
            }
            return equals;
        }

        private IList<TProperty> GetSmaller(IList<TProperty> collection, IList<TProperty> other)
        {
            var count = Math.Min(collection.Count, other.Count);
            if (count == collection.Count)
            {
                return collection;
            }
            return other;
        }

        private ICollection<TProperty> GetBigger(IList<TProperty> collection, IList<TProperty> other)
        {
            var count = Math.Max(collection.Count, other.Count);
            if (count == collection.Count)
            {
                return collection;
            }
            return other;
        }

        public int GetHashCode(TModel obj)
        {
            var hashCode = -2129598428;
            foreach (var item in GetProperty(obj))
            {
                hashCode = hashCode * -1521134295 + ItemComparer.GetHashCode(item);
            }
            return hashCode;
        }

        public int LevenstheinDistance(TModel model, TModel other)
        {
            var distance = 0;
            var otherList = GetProperty(other);
            foreach (var item in GetProperty(model))
            {
                if (otherList.Contains(item) == false) { continue; }

                var idx = otherList.IndexOf(item);
                distance += ItemComparer.LevenstheinDistance(item, otherList[idx]);
            }
            return distance;
        }

        public string LevenstheinDistanceAsString(TModel model, TModel other)
        {
            var distance = LevenstheinDistance(model, other);
            return LevenstheinHelper.LevenstheinAsString(distance);
        }

        public abstract IList<TProperty> GetProperty(TModel model);

        public int Compare(TModel model, TModel other)
        {
            return LevenstheinDistance(model, other);
        }
    }
}
