using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Utilities.System.FileSystem
{
    public static class FileUtils
    {
        public static string GetFirstFile(string directory, string pattern = "*.*", bool recursive = true)
        {
            var firstFile = DirUtils.MyDocuments;
            var files = GetFiles(directory, pattern, recursive);
            if (files.Any())
            {
                firstFile = files.First();
            }
            return firstFile;
        }

        public static IEnumerable<string> GetFiles(string directory, string pattern = "*.*", bool recursive = true)
        {
            var option = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            directory = DirUtils.GetDirectory(directory);
            return Directory.GetFiles(directory, pattern, option);
        }
    }
}
