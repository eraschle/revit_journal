using System.IO;
using static System.Environment;

namespace Utilities.System
{
    public static class DirUtils
    {
        public static string GetDirectory(string directory,
            SpecialFolder defaultFolder = SpecialFolder.MyDocuments)
        {
            if (string.IsNullOrWhiteSpace(directory)
                || Directory.Exists(directory) == false)
            {
                directory = GetFolderPath(defaultFolder);
            }
            return directory;
        }

        public static string MyDocuments
        {
            get { return GetFolderPath(SpecialFolder.MyDocuments); }
        }
    }
}
