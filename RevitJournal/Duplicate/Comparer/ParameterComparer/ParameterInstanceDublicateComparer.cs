using DataSource.Model.Family;

namespace RevitJournal.Duplicate.Comparer.ParameterComparer
{
    public class ParameterInstanceDublicateComparer : ADuplicateComparer<Parameter>
    {
        private const Parameter Model = null;
        private const string ModelPropertyName = nameof(Model.IsInstance);

        public ParameterInstanceDublicateComparer(bool dublicateComparer, string displayName = ModelPropertyName)
            : base(dublicateComparer, ModelPropertyName, displayName) { }


        public override string GetProperty(Parameter model)
        {
            return model.IsInstance.ToString();
        }
    }
}
