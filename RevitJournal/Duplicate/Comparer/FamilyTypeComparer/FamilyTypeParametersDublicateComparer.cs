using DataSource.Model.Metadata;
using RevitJournal.Duplicate.Comparer.ParameterComparer;
using System.Collections.Generic;

namespace RevitJournal.Duplicate.Comparer.FamilyTypeComparer
{
    public class FamilyTypeParametersDublicateComparer : ACollectionDuplicateComparer<FamilyType, Parameter>
    {
        public static readonly ParameterDuplicateComparer ParameterComparer
            = new ParameterDuplicateComparer(
                new List<IDuplicateComparer<Parameter>>
                {
                    new ParameterIdDublicateComparer(false),
                    new ParameterNameDublicateComparer(true),
                    new ParameterValueDublicateComparer(false),
                    new ParameterValueTypeDublicateComparer(false),
                    new ParameterParameterTypeDublicateComparer(true),
                    new ParameterInstanceDublicateComparer(true),
                    new ParameterReadOnlyDublicateComparer(true)
                }
            );

        private const FamilyType Model = null;
        private const string ModelPropertyName = nameof(Model.Parameters);

        public FamilyTypeParametersDublicateComparer(bool dublicateComparer, string displayName = ModelPropertyName)
            : base(dublicateComparer, ModelPropertyName, displayName, ParameterComparer) { }

        public override IList<Parameter> GetProperty(FamilyType model)
        {
            return model.Parameters;
        }
    }
}
