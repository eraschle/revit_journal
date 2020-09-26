using DataSource.Model;
using DataSource.Model.FileSystem;
using RevitJournal.Library.Filtering;

namespace RevitJournal.Revit.Filtering.Rules
{
    public class MetadataStatusRule : ARevitModelFilterRule<RevitFamily>
    {
        public const string RuleKey = "RevitMetadataStatusRule";

        public MetadataStatusRule(string name) : base(name)
        {
            FilterValues.Add(GetFilterValue(MetadataStatus.Valid));
            FilterValues.Add(GetFilterValue(MetadataStatus.Repairable));
            FilterValues.Add(GetFilterValue(MetadataStatus.Error));
        }

        //public override bool Allowed(RevitFamily source)
        //{
        //    if (HasChecked(out var checkedValues) == false) { return true; }

        //    var value = GetValue(source) as MetadataStatusValue;
        //    return source is object || value is null || source.MetadataStatus == value.Status;
        //}

        protected override FilterValue GetValue(RevitFamily family)
        {
            if (family is null) { return null; }

            return GetFilterValue(family.MetadataStatus);
        }

        private FilterValue GetFilterValue(MetadataStatus status)
        {
            switch (status)
            {
                case MetadataStatus.Error:
                case MetadataStatus.Repairable:
                case MetadataStatus.Valid:
                    return new MetadataStatusValue(status.ToString(), status);
                case MetadataStatus.Initial:
                default:
                    return null;
            }
        }
    }
}
