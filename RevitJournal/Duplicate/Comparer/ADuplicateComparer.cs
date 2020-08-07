namespace RevitJournal.Duplicate.Comparer
{
    public abstract class ADuplicateComparer<TModel> : IDuplicateComparer<TModel>
    {
        public bool UseComparer { get; set; } = false;

        public string PropertyName { get; private set; }

        public string DisplayName { get; private set; }

        protected ADuplicateComparer(bool dublicateComparer, string propertyName, string displayName)
        {
            UseComparer = dublicateComparer;
            PropertyName = propertyName;
            DisplayName = displayName;
        }

        public bool Equals(TModel model, TModel other)
        {
            return model != null && other != null && GetProperty(model).Equals(GetProperty(other));
        }

        public int GetHashCode(TModel obj)
        {
            return GetProperty(obj).GetHashCode();
        }

        public int LevenstheinDistance(TModel model, TModel other)
        {
            return LevenstheinHelper.ComputeLevensthein(GetProperty(model), GetProperty(other));
        }

        public string LevenstheinDistanceAsString(TModel model, TModel other)
        {
            var distance = LevenstheinDistance(model, other);
            return LevenstheinHelper.LevenstheinAsString(distance);
        }

        public abstract string GetProperty(TModel model);

        public int Compare(TModel model, TModel other)
        {
            return LevenstheinDistance(model, other);
        }

        public bool AreEmpty(TModel model, TModel other)
        {
            return string.IsNullOrWhiteSpace(GetProperty(model))
                && string.IsNullOrWhiteSpace(GetProperty(other));
        }
    }
}
