using DataSource.Models.FileSystem;
using DataSource.Models.Product;
using RevitJournal.Library.Filtering;

namespace RevitJournal.Revit.Filtering.Rules
{
    public class ProductRule : ARevitModelFilterRule<RevitFamilyFile>
    {
        public const string RuleKey = "RevitProduct";

        public ProductRule(string name) : base(name) { }

        protected override FilterValue GetValue(RevitFamilyFile family)
        {
            return HasProduct(family, out var product)
                ? new FilterValue(product.ProductName)
                : null;
        }

        private bool HasProduct(RevitFamilyFile family, out RevitApp product)
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
