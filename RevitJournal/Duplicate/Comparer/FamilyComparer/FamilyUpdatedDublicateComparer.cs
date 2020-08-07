using DataSource.Helper;
using DataSource.Model.Family;

namespace RevitJournal.Duplicate.Comparer.FamilyComparer
{
    public class FamilyUpdatedDublicateComparer : ADuplicateComparer<Family>
    {
        private const Family Model = null;
        private const string ModelPropertyName = nameof(Model.Updated);

        public FamilyUpdatedDublicateComparer(bool dublicateComparer, string displayName = ModelPropertyName)
            : base(dublicateComparer, ModelPropertyName, displayName) { }


        public override string GetProperty(Family model)
        {
            return DateHelper.AsString(model.Updated);
        }
    }
}
