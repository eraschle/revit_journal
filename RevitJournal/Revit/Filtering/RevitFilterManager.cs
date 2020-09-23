﻿using DataSource.Model.FileSystem;
using RevitJournal.Library.Filtering;
using RevitJournal.Revit.Filtering.Rules;

namespace RevitJournal.Revit.Filtering
{
    public class RevitFilterManager : FilterManager<RevitFamily>
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
            AddRule(OmniClassRule.RuleKey, new OmniClassRule("Omni Classes"));
            AddRule(MetadataStatusRule.RuleKey, new MetadataStatusRule("Metadata Status"));
            AddRule(BasicComponentRule.RuleKey, new BasicComponentRule("Basic Component"));
            AddRule(ParameterRule.RuleKey, new ParameterRule("Family Parameter"));
        }

        public bool DirectoryFilter(DirectoryNode directory)
        {
            if (directory is null) { return true; }
            if (HasFilters() == false) { return true; }

            foreach (var folder in directory.GetSubfolders<RevitFamilyFile>())
            {
                DirectoryFilter(folder);
            }
            return true;
        }
    }
}
