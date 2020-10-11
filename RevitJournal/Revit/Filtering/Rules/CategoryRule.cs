using DataSource.Models.Catalog;
using DataSource.Models.FileSystem;
using RevitJournal.Library.Filtering;

namespace RevitJournal.Revit.Filtering.Rules
{
    public class CategoryRule : ARevitModelFilterRule<RevitFamilyFile>
    {
        public const string RuleKey = "RevitCategories";

        public CategoryRule(string name) : base(name) { }

        protected override FilterValue GetValue(RevitFamilyFile family)
        {
            return HasCategory(family, out var category)
                ? new FilterValue(category)
                : null;
        }

        private bool HasCategory(RevitFamilyFile family, out Category category)
        {
            category = null;
            if(family is object && family.Metadata is object)
            {
                category = family.Metadata.Category;
            }
            return category is object;
        }
    }
}
