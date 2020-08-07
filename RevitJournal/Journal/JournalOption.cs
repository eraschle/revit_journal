using DataSource.Helper;
using DataSource.Model.FileSystem;
using DataSource.Model.Product;
using RevitJournal.Revit;
using System;
using System.IO;

namespace RevitJournal.Journal
{
    public class JournalOption
    {
        private static string CreateDefaultBackupSuffix()
        {
            return Constant.Underline + DateTime.Now.ToShortDateString()
                .Replace(Constant.PointChar, Constant.MinusChar)
                .Replace(Constant.SlashChar, Constant.MinusChar);
        }

        public static TimeSpan MinimumTimeout { get { return TimeSpan.FromMinutes(1); } }

        public static TimeSpan DefaultTimeout { get { return MinimumTimeout; } }

        public static TimeSpan MaximumTimeout { get { return TimeSpan.FromMinutes(20); } }

        public TimeSpan TaskTimeout { get; set; } = DefaultTimeout;

        public RevitApp RevitApp { get; set; }

        public bool LogResults { get; set; } = true;

        public bool LogSuccess { get; set; } = false;

        public bool LogError { get; set; } = true;

        public RevitFile GetBackupFile(RevitFile revitFile)
        {
            if (BackupRevitFile == false || revitFile is null)
            {
                return null;
            }
            var backupFile = revitFile;
            if(string.IsNullOrEmpty(BackupSubFolder) == false)
            {
                var withSubFolder = Path.Combine(revitFile.ParentFolder, BackupSubFolder);
                if(Directory.Exists(withSubFolder) == false)
                {
                    Directory.CreateDirectory(withSubFolder);
                }
                backupFile = revitFile.ChangeDirectory<RevitFile>(withSubFolder);
            }
            var backupFileName = revitFile.Name + BackupSuffix;
            return backupFile.ChangeFileName<RevitFile>(backupFileName);
        }

        public bool BackupRevitFile { get; set; } = false;

        private string _BackupSuffix = CreateDefaultBackupSuffix();
        public string BackupSuffix
        {
            get { return _BackupSuffix; }
            set
            {
                _BackupSuffix = value;
                if (_BackupSuffix.StartsWith(Constant.Underline, StringComparison.CurrentCulture) == false)
                {
                    _BackupSuffix = string.Concat(Constant.Underline, _BackupSuffix);
                }
            }
        }

        private string _BackupSubFolder = string.Empty;
        public string BackupSubFolder
        {
            get { return _BackupSubFolder; }
            set
            {
                _BackupSubFolder = value;
                if (_BackupSubFolder.Contains(Constant.Space))
                {
                    _BackupSubFolder = _BackupSubFolder.Replace(Constant.Space, Constant.Underline);
                }
            }
        }
    }
}
