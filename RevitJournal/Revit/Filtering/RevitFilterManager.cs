using DataSource.Model.FileSystem;
using RevitJournal.Library.Filtering;
using RevitJournal.Revit.Filtering.Rules;

namespace RevitJournal.Revit.Filtering
{
    public class RevitFilterManager : FilterManager<RevitFamily>
    {
        public RevitFilterManager()
        {
            AddRule(CategoryRule.RuleKey, new CategoryRule("Categories"));
            AddRule(ProductRule.RuleKey, new ProductRule("Product Versions"));
            AddRule(OmniClassRule.RuleKey, new OmniClassRule("Omni Classes"));
            AddRule(MetadataStatusRule.RuleKey, new MetadataStatusRule("Metadata Status"));
            AddRule(BasicComponentRule.RuleKey, new BasicComponentRule("Basic Component"));
            AddRule(ParameterRule.RuleKey, new ParameterRule("Basic Component"));
        }

        //public bool IsDataNotExist(RevitFamily family)
        //{
        //    return MetadatFileNotExist && family is object && family.HasFileMetadata == false;
        //}

        public bool DirectoryFilter(RevitDirectory directory)
        {
            if (directory is null) { return true; }

            foreach (var folder in directory.Subfolder)
            {
                DirectoryFilter(folder);
            }
            return true;
        }
    }
}
