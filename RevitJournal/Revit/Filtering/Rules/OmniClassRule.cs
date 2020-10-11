using DataSource.Models.Catalog;
using DataSource.Models.FileSystem;
using RevitJournal.Library.Filtering;

namespace RevitJournal.Revit.Filtering.Rules
{
    public class OmniClassRule : ARevitModelFilterRule<RevitFamilyFile>
    {
        public const string RuleKey = "RevitOmniClass";

        internal const string NoValue = "No Value";

        public OmniClassRule(string name, string defaultValue) : base(name, defaultValue) { }

        protected override FilterValue GetValue(RevitFamilyFile family)
        {
            return HasOmniClass(family, out var omniClass)
                ? new FilterValue(omniClass)
                : new FilterValue(NoValue);
        }

        private bool HasOmniClass(RevitFamilyFile family, out OmniClass omniClass)
        {
            omniClass = null;
            if (family is object && family.Metadata is object)
            {
                omniClass = family.Metadata.OmniClass;
            }
            return omniClass is object;
        }
    }
}
