using DataSource.Models.FileSystem;
using RevitJournal.Duplicate.Comparer;
using RevitJournal.Library;
using System.Collections.Generic;
using DataSource.Model.Metadata;

namespace RevitJournal.Duplicate
{
    public static class DuplicateManager
    {
        public static Dictionary<Family, HashSet<Family>> Create(LibraryManager manager, FamilyDuplicateComparer comparer)
        {
            var added = new HashSet<Family>();

            var duplicateMap = new Dictionary<Family, HashSet<Family>>(comparer);
            var libraryFiles = manager.CheckedValidFiles();
            for (var idx = 0; idx < libraryFiles.Count; idx++)
            {
                var family = libraryFiles[idx];
                var metadata = family.Metadata;
                if (metadata is null || added.Contains(metadata)) { continue; }

                if (duplicateMap.ContainsKey(metadata) == false)
                {
                    duplicateMap.Add(metadata, new HashSet<Family>());
                    added.Add(metadata);
                }
                for (var idxDuplicate = idx + 1; idxDuplicate < libraryFiles.Count; idxDuplicate++)
                {
                    var duplicateFamily = libraryFiles[idxDuplicate];
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
