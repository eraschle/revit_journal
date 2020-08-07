using DataSource.Model.Family;

namespace RevitJournal.Duplicate.Comparer.FamilyComparer
{
    public class FamilyNameDublicateComparer : ADuplicateComparer<Family>
    {
        private const Family Model = null;
        private const string NamePropertyName = nameof(Model.Name);
        public FamilyNameDublicateComparer(bool dublicateComparer, string displayName = NamePropertyName)
            : base(dublicateComparer, NamePropertyName, displayName) { }

        public override string GetProperty(Family model)
        {
            return model.Name;
        }
    }
}
