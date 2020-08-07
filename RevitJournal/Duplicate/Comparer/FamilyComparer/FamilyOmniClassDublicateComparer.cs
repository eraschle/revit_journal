using DataSource.Model.Family;

namespace RevitJournal.Duplicate.Comparer.FamilyComparer
{
    public class FamilyOmniClassDublicateComparer : ADuplicateComparer<Family>
    {
        private const Family Model = null;
        private const string ModelPropertyName = nameof(Model.OmniClass);

        public FamilyOmniClassDublicateComparer(bool dublicateComparer, string displayName = ModelPropertyName)
            : base(dublicateComparer, ModelPropertyName, displayName) { }

        public override string GetProperty(Family model)
        {
            if(model.HasOmniClass(out var omniClass) == false) { return string.Empty; }

            return omniClass.NumberAndName;
        }
    }
}
