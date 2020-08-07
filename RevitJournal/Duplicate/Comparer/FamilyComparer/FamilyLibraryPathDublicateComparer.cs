using DataSource.Model.Family;

namespace RevitJournal.Duplicate.Comparer.FamilyComparer
{
    public class FamilyLibraryPathDublicateComparer : ADuplicateComparer<Family>
    {
        private const Family Model = null;
        private const string ModelPropertyName = nameof(Model.LibraryPath);
        public FamilyLibraryPathDublicateComparer(bool dublicateComparer, string displayName = ModelPropertyName)
            : base(dublicateComparer, ModelPropertyName, displayName) { }

        public override string GetProperty(Family model)
        {
            return model.LibraryPath;
        }
    }
}
