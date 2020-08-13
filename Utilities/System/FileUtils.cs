using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Utilities.System
{
    public static class FileUtils
    {
        public static string GetFirstFile(
            string directory, string pattern = "*.*", SearchOption option = SearchOption.AllDirectories)
        {
            var firstFile = GetFiles(directory, pattern, option).FirstOrDefault();
            if (firstFile is null)
            {
                firstFile = DirUtils.MyDocuments;
            }
            return firstFile;
        }

        public static IEnumerable<string> GetFiles(
            string directory, string pattern = "*.*", SearchOption option = SearchOption.AllDirectories)
        {
            directory = DirUtils.GetDirectory(directory);
            return Directory.GetFiles(directory, pattern, option);
        }
    }
}
