using DataSource.Model.Metadata;
using RevitJournal.Duplicate.Comparer.ParameterComparer;
using System.Collections.Generic;

namespace RevitJournal.Duplicate.Comparer.FamilyComparer
{
    public class FamilyParametersDublicateComparer : ACollectionDuplicateComparer<Family, Parameter>
    {
        public static readonly ParameterDuplicateComparer ParameterComparer 
            = new ParameterDuplicateComparer(
                new List<IDuplicateComparer<Parameter>>
                {
                    new ParameterIdDublicateComparer(false),
                    new ParameterNameDublicateComparer(true),
                    new ParameterValueDublicateComparer(true),
                    new ParameterValueTypeDublicateComparer(false),
                    new ParameterParameterTypeDublicateComparer(false),
                }
            );

        private const Family Model = null;
        private const string ModelPropertyName = nameof(Model.Parameters);

        public FamilyParametersDublicateComparer(bool dublicateComparer, string displayName = ModelPropertyName)
            : base(dublicateComparer, ModelPropertyName, displayName, ParameterComparer) { }

        public override IList<Parameter> GetProperty(Family model)
        {
            return model.Parameters;
        }
    }
}
