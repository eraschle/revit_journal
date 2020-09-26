using System;
using System.IO;
using System.Linq;
using static System.Environment;

namespace Utilities.System.FileSystem
{
    public static class DirUtils
    {
        public static string MyDocuments
        {
            get { return GetFolderPath(SpecialFolder.MyDocuments); }
        }

        public static string GetDirectory(string directory, SpecialFolder fallback = SpecialFolder.MyDocuments)
        {
            if (string.IsNullOrWhiteSpace(directory)
                || Directory.Exists(directory) == false)
            {
                directory = GetFolderPath(fallback);
            }
            return directory;
        }

        public static string RemoveSlases(string path)
        {
            if (StringUtils.Starts(path, Constant.PathSeparator))
            {
                path = path.Substring(1);
            }

            if (StringUtils.Ends(path, Constant.PathSeparator))
            {
                path = path.Remove(path.Length - 1);
            }
            return path;
        }

        public static string GetParentPath(string path)
        {
            var lastName = GetLastPathName(path);
            return path.Remove(path.Length - (lastName.Length + 1));
        }

        public static string GetLastPathName(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) { throw new ArgumentNullException(nameof(path)); }

            var split = path.Split(Constant.PathSeparatorChar);
            if (split.Length == 0)
            {
                throw new ArgumentException($"Path not valid: {path}");
            }
            return split.Last();
        }
    }
}
