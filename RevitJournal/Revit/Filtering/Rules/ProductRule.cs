using DataSource.Model.FileSystem;
using DataSource.Model.Product;
using RevitJournal.Library.Filtering;

namespace RevitJournal.Revit.Filtering.Rules
{
    public class ProductRule : ARevitModelFilterRule<RevitFamily>
    {
        public const string RuleKey = "RevitProduct";

        public ProductRule(string name) : base(name) { }

        protected override FilterValue GetValue(RevitFamily family)
        {
            return HasProduct(family, out var product)
                ? new FilterValue(product.ProductName)
                : null;
        }

        private bool HasProduct(RevitFamily family, out RevitApp product)
        {
            product = null;
            if(family is object && family.Metadata is object)
            {
                product = family.Metadata.Product;
            }
            return product is object;
        }
    }
}
