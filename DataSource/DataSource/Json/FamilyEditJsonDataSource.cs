using DataSource.Model.FileSystem;

namespace DataSource.DataSource.Json
{
    public class FamilyEditJsonDataSource : FamilyJsonDataSource
    {
        public const string Suffix = "edited";

        public override void SetFamilyFile(RevitFamilyFile fileNode)
        {
            base.SetFamilyFile(fileNode);
            FileNode.AddSuffixes(Suffix);
        }
    }
}