using DataSource.Model.Family;

namespace RevitJournal.Duplicate.Comparer.FamilyComparer
{
    public class FamilyDisplayNameDublicateComparer : ADuplicateComparer<Family>
    {
        private const Family Model = null;
        private const string ModelPropertyName = nameof(Model.DisplayName);
        public FamilyDisplayNameDublicateComparer(bool dublicateComparer, string displayName = ModelPropertyName)
            : base(dublicateComparer, ModelPropertyName, displayName) { }

        public override string GetProperty(Family model)
        {
            return model.DisplayName;
        }
    }
}
