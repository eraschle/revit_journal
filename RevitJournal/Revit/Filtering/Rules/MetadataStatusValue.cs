using DataSource.Models;
using RevitJournal.Library.Filtering;

namespace RevitJournal.Revit.Filtering.Rules
{
    public class MetadataStatusValue : FilterValue
    {
        public MetadataStatus Status { get; set; }

        public MetadataStatusValue(string name, MetadataStatus status): base(name)
        {
            Status = status;
        }
    }
}
