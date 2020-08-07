using DataSource.Helper;
using DataSource.Model.FileSystem;
using System.IO;
using System.Linq;

namespace DataSource.Model.Product
{
    public class RevitAppFile : AFile
    {
        public const string FileExtension = "exe";
        public static string FileName { get; } = FileNameWithExtension.Replace(Constant.Point, string.Empty)
                                                                      .Replace(FileExtension, string.Empty);

        internal const string FileNameWithExtension = "Revit.exe";

        private int _Version = 0;
        public int Version
        {
            get
            {
                if (_Version == 0)
                {
                    _Version = GetRevitVersion();
                }
                return _Version;
            }
        }

        private int GetRevitVersion()
        {
            var folders = ParentFolder.Split(Path.DirectorySeparatorChar);
            var parentFolder = folders.LastOrDefault();
            var versionIdx = parentFolder.LastIndexOf(Constant.SpaceChar);
            return int.Parse(parentFolder.Remove(0, versionIdx));
        }

        protected override string GetExtension()
        {
            return FileExtension;
        }

        protected override string GetTypeName()
        {
            return nameof(RevitAppFile);
        }
    }
}
