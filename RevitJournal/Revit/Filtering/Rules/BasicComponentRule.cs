using DataSource.Model.Family;
using DataSource.Model.FileSystem;
using RevitJournal.Library.Filtering;

namespace RevitJournal.Revit.Filtering.Rules
{
    public class BasicComponentRule : ARevitModelFilterRule<RevitFamily>
    {
        public const string RuleKey = "RevitBasicComponent";

        public BasicComponentRule(string name) : base(name) { }

        protected override FilterValue GetValue(RevitFamily family)
        {
            return HasBasicComponent(family, out var basicComponent)
                ? new FilterValue(basicComponent)
                : null;
        }

        private bool HasBasicComponent(RevitFamily family, out Parameter basicComponent)
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
