namespace RevitJournal.Duplicate.Comparer
{
    public interface IDuplicateComparer<TModel> : ILevenstheinComparer<TModel>
    {
        bool UseComparer { get; set; }

        string PropertyName { get; }
        
        string DisplayName { get; }
    }
}
