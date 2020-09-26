using DataSource.Helper;
using DataSource.Model.Family;
using System;
using Utilities.System;

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
            if(model is null) { throw new ArgumentNullException(nameof(model)); }

            return DateUtils.AsString(model.Updated);
        }
    }
}
