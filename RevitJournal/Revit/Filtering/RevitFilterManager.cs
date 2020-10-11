using DataSource.Models.FileSystem;
using RevitJournal.Library.Filtering;
using RevitJournal.Revit.Filtering.Rules;

namespace RevitJournal.Revit.Filtering
{
    public class RevitFilterManager : FilterManager<RevitFamilyFile>
    {
        private static RevitFilterManager instance;
        public static RevitFilterManager Instance
        {
            get
            {
                if(instance is null)
                {
                    instance = new RevitFilterManager();
                }
                return instance;
            }
        }

        private RevitFilterManager()
        {
            AddRule(CategoryRule.RuleKey, new CategoryRule("Categories"));
            AddRule(ProductRule.RuleKey, new ProductRule("Product Versions"));
            AddRule(OmniClassRule.RuleKey, new OmniClassRule("Omni Classes", OmniClassRule.NoValue));
            AddRule(MetadataStatusRule.RuleKey, new MetadataStatusRule("Metadata Status"));
            AddRule(BasicComponentRule.RuleKey, new BasicComponentRule("Basic Component"));
            AddRule(ParameterRule.RuleKey, new ParameterRule("Family Parameter"));
        }

        public bool DirectoryFilter(DirectoryNode directory)
        {
            if (directory is null) { return true; }
            if (HasFilters() == false) { return true; }

            foreach (var folder in directory.GetDirectories<RevitFamilyFile>(false))
            {
                DirectoryFilter(folder);
            }
            return true;
        }
    }
}
