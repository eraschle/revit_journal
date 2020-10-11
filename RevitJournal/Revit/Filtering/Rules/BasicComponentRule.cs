using DataSource.Models.FileSystem;
using RevitJournal.Library.Filtering;
using DataSource.Model.Metadata;

namespace RevitJournal.Revit.Filtering.Rules
{
    public class BasicComponentRule : ARevitModelFilterRule<RevitFamilyFile>
    {
        public const string RuleKey = "RevitBasicComponent";

        public BasicComponentRule(string name) : base(name) { }

        protected override FilterValue GetValue(RevitFamilyFile family)
        {
            return HasBasicComponent(family, out var basicComponent)
                ? new FilterValue(basicComponent)
                : null;
        }

        private bool HasBasicComponent(RevitFamilyFile family, out Parameter basicComponent)
        {
            basicComponent = null;
            if(family is object && family.Metadata is object )
            {
                family.Metadata.HasByName(Family.BasicComponent, out basicComponent);
            }
            return basicComponent is object;
        }
    }
}
