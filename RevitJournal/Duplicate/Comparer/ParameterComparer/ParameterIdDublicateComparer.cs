using DataSource.Model.Metadata;

namespace RevitJournal.Duplicate.Comparer.ParameterComparer
{
    public class ParameterIdDublicateComparer : ADuplicateComparer<Parameter>
    {
        private const Parameter Model = null;
        private const string ModelPropertyName = nameof(Model.Id);

        public ParameterIdDublicateComparer(bool dublicateComparer, string displayName = ModelPropertyName)
            : base(dublicateComparer, ModelPropertyName, displayName) { }

        public override string GetProperty(Parameter model)
        {
            return model.Id;
        }
    }
}
