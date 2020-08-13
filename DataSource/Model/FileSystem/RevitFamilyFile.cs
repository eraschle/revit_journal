using DataSource.Helper;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DataSource.Model.FileSystem
{
    public class RevitFamilyFile : AFile
    {
        public const string FileExtension = "rfa";

        private static readonly Regex RevitFamilyBackup = new Regex(@"\\(\d{4})\\");

        protected override string GetExtension()
        {
            return FileExtension;
        }

        protected override string GetTypeName()
        {
            return nameof(RevitFamilyFile);
        }


        [JsonIgnore]
        public bool IsBackup { get { return IsBackupFile(); } }

        private bool IsBackupFile()
        {
            var backupSplit = Name.Split(Constant.PointChar);
            if (backupSplit.Length == 1) { return false; }

            var backup = backupSplit.LastOrDefault();
            if (string.IsNullOrWhiteSpace(backup)) { return false; }

            return RevitFamilyBackup.IsMatch(backup);
        }

        public IEnumerable<RevitFamilyFile> Backups
        {
            get
            {
                var search = string.Concat(Name, Constant.Point, Constant.Star, Constant.Point, Extension);
                return Directory.GetFiles(ParentFolder, search, SearchOption.TopDirectoryOnly)
                    .Select(path => new RevitFamilyFile { FullPath = path })
                    .Where(rvt => rvt.IsBackup);
            }
        }
    }
}
