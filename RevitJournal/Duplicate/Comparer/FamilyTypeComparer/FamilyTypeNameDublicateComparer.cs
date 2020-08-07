using DataSource.Model.Family;

namespace RevitJournal.Duplicate.Comparer.FamilyTypeComparer
{
    public class FamilyTypeNameDublicateComparer : ADuplicateComparer<FamilyType>
    {
        private const FamilyType Model = null;
        private const string ModelPropertyName = nameof(Model.Name);

        public FamilyTypeNameDublicateComparer(bool dublicateComparer, string displayName = ModelPropertyName)
            : base(dublicateComparer, ModelPropertyName, displayName) { }


        public override string GetProperty(FamilyType model)
        {
            return model.Name;
        }
    }
}
