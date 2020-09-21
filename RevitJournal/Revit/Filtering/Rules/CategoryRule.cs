using DataSource.Model.Catalog;
using DataSource.Model.FileSystem;
using RevitJournal.Library.Filtering;

namespace RevitJournal.Revit.Filtering.Rules
{
    public class CategoryRule : ARevitModelFilterRule<RevitFamily>
    {
        public const string RuleKey = "RevitCategories";

        public CategoryRule(string name) : base(name) { }

        protected override FilterValue GetValue(RevitFamily family)
        {
            return HasCategory(family, out var category)
                ? new FilterValue(category)
                : null;
        }

        private bool HasCategory(RevitFamily family, out Category category)
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
