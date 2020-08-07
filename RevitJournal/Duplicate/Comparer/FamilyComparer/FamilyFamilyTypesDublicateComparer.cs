using DataSource.Model.Family;
using System.Collections.Generic;

namespace RevitJournal.Duplicate.Comparer.FamilyComparer
{
    public class FamilyFamilyTypesDublicateComparer : ACollectionDuplicateComparer<Family, FamilyType>
    {
        private const Family Model = null;
        private const string ModelPropertyName = nameof(Model.FamilyTypes);
        public FamilyFamilyTypesDublicateComparer(bool dublicateComparer, string displayName = ModelPropertyName)
            : base(dublicateComparer, ModelPropertyName, displayName, new FamilyTypeDuplicateComparer()) { }

        public override IList<FamilyType> GetProperty(Family model)
        {
            return model.FamilyTypes;
        }
    }
}
