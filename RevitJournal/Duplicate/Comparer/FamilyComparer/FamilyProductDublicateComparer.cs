using DataSource.Model.Metadata;

namespace RevitJournal.Duplicate.Comparer.FamilyComparer
{
    public class FamilyProductDublicateComparer : ADuplicateComparer<Family>
    {
        private const Family Model = null;
        private const string ModelPropertyName = nameof(Model.Product);

        public FamilyProductDublicateComparer(bool dublicateComparer, string displayName = ModelPropertyName)
            : base(dublicateComparer, ModelPropertyName, displayName) { }


        public override string GetProperty(Family model)
        {
            if (model.HasProduct(out var product) == false) { return string.Empty; }

            return product.ProductName;
        }
    }
}
