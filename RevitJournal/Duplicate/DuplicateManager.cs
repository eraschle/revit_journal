using DataSource.Model.Family;
using DataSource.Model.FileSystem;
using RevitJournal.Duplicate.Comparer;
using System.Collections.Generic;

namespace RevitJournal.Duplicate
{
    public static class DuplicateManager
    {
        public static Dictionary<Family, HashSet<Family>> Create(IList<RevitFamily> families, FamilyDuplicateComparer comparer)
        {
            var added = new HashSet<Family>();

            var duplicateMap = new Dictionary<Family, HashSet<Family>>(comparer);
            for (var idx = 0; idx < families.Count; idx++)
            {
                var family = families[idx];
                var metadata = family.Metadata;
                if (metadata is null || added.Contains(metadata)) { continue; }

                if (duplicateMap.ContainsKey(metadata) == false)
                {
                    duplicateMap.Add(metadata, new HashSet<Family>());
                    added.Add(metadata);
                }
                for (var idxDuplicate = idx + 1; idxDuplicate < families.Count; idxDuplicate++)
                {
                    var duplicateFamily = families[idxDuplicate];
                    var duplicateMetadata = duplicateFamily.Metadata;

                    if (duplicateMetadata is null || added.Contains(duplicateMetadata)) { continue; }
                    if (duplicateMap.ContainsKey(duplicateMetadata) == false) { continue; }

                    duplicateMap[duplicateMetadata].Add(duplicateMetadata);
                    added.Add(duplicateMetadata);
                }
            }
            return duplicateMap;
        }
    }
}
