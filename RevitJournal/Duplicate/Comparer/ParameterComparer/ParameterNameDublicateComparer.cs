using DataSource.Model.Metadata;

namespace RevitJournal.Duplicate.Comparer.ParameterComparer
{
    public class ParameterNameDublicateComparer : ADuplicateComparer<Parameter>
    {
        private const Parameter Model = null;
        private const string ModelPropertyName = nameof(Model.Name);

        public ParameterNameDublicateComparer(bool dublicateComparer, string displayName = ModelPropertyName)
            : base(dublicateComparer, ModelPropertyName, displayName) { }


        public override string GetProperty(Parameter model)
        {
            return model.Name;
        }
    }
}
