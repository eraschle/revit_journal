using DataSource.Models.FileSystem;

namespace DataSource.DataSource.Json
{
    public class FamilyEditJsonDataSource : FamilyJsonDataSource
    {
        public const string Suffix = "edited";

        public override void SetFile(RevitFamilyFile fileNode)
        {
            base.SetFile(fileNode);
            FileNode.AddSuffixes(Suffix);
        }
    }
}