using DataSource.Model.Metadata;

namespace RevitJournal.Duplicate.Comparer.ParameterComparer
{
    public class ParameterValueDublicateComparer : ADuplicateComparer<Parameter>
    {
        private const Parameter Model = null;
        private const string ModelPropertyName = nameof(Model.Value);

        public ParameterValueDublicateComparer(bool dublicateComparer, string displayName = ModelPropertyName)
            : base(dublicateComparer, ModelPropertyName, displayName) { }


        public override string GetProperty(Parameter model)
        {
            return model.Value;
        }
    }
}
