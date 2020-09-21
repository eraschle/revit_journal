using DataSource.Model.Catalog;
using DataSource.Model.FileSystem;
using RevitJournal.Library.Filtering;

namespace RevitJournal.Revit.Filtering.Rules
{
    public class OmniClassRule : ARevitModelFilterRule<RevitFamily>
    {
        public const string RuleKey = "RevitOmniClass";

        internal const string DefaultNoValue = "No Value";

        public OmniClassRule(string name) : base(name) { }

        protected override FilterValue GetValue(RevitFamily family)
        {
            return HasOmniClass(family, out var omniClass)
                ? new FilterValue(omniClass)
                : new FilterValue(DefaultNoValue);
        }

        private bool HasOmniClass(RevitFamily family, out OmniClass omniClass)
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
