using DataSource.Helper;
using DataSource.Model.FileSystem;
using System.Collections.Generic;
using System.IO;

namespace RevitJournal.Journal.Command.Document
{
    public class DocumentSaveAsCommand : AJournalCommand
    {
        public const string InfoParameter = "Save As Path Model";
        public const string AddFolderPosition = "Add Folder At End";
        public const string RootPath = "Library Folder [Current]";
        public const string NewRootPath = "Library Folder [New]";

        private const int FileSuffixIdx = 1;
        private const int BackupFolderIdx = 2;
        private const int AddToEndIdx = 3;
        private const int RootPathIdx = 4;
        private const int NewRootPathIdx = 5;

        public const string LibraryRootName = "[Library Folder]";
        public const string NewLibraryRootName = "[New Library Folder]";
        public const string LibraryPathName = "[Library Path]";
        public const string SaveAsFolderName = "[Save As Folder]";
        public const string RevitFileName = "[File Name]";
        public const string SaveAsSuffixName = "[Save As Suffix]";

        public DocumentSaveAsCommand() : base("Save As")
        {
            Parameters.Add(new CommandParameterInfo(InfoParameter, CreateSaveAsPath));
            Parameters.Add(new DocumentSaveAsParameterFile());
            Parameters.Add(new DocumentSaveAsParameterFolder());
            Parameters.Add(new CommandParameter(AddFolderPosition, JournalParameterType.Boolean));
            Parameters.Add(new CommandParameter(RootPath, JournalParameterType.Folder, false));
            Parameters.Add(new CommandParameter(NewRootPath, JournalParameterType.Folder));
        }

        public override IEnumerable<string> Commands
        {
            get
            {
                return new string[] {
                    JournalCommandBuilder.Build("Ribbon", "ID_REVIT_SAVE_AS_FAMILY"),
                    string.Concat("Jrn.Data \"File Name\" , \"IDOK\", \"", RevitFile.FullPath, "\"") };
            }
        }

        private RevitFile RevitFile { get; set; }

        public override void PostExecutionTask(JournalResult result)
        {
            if (result is null) { return; }

            if (RevitFile.Exist)
            {
                result.Result = RevitFile;
            }
        }

        public override void PreExecutionTask(RevitFamily family)
        {
            var revitFile = family.RevitFile;
            var currentRootPath = GetRootPath();
            var rootPath = currentRootPath;
            if (HasNewRootPath(out var newRoot))
            {
                rootPath = newRoot;
            }
            var backupPath = revitFile.ParentFolder;
            if (HasBackupFolderName(out var backupFolder))
            {
                var libraryPath = revitFile.ParentFolder.Replace(currentRootPath, string.Empty);
                backupPath = Path.Combine(rootPath, backupFolder, libraryPath);
                if (IsAddFolderAtEnd())
                {
                    backupPath = Path.Combine(rootPath, libraryPath, backupFolder);
                }

                if (libraryPath.StartsWith(Constant.BackSlash))
                {
                    libraryPath = libraryPath.Substring(1);
                }
            }
            if (Directory.Exists(backupPath) == false)
            {
                Directory.CreateDirectory(backupPath);
            }
            RevitFile = revitFile.ChangeDirectory<RevitFile>(backupPath);
            if (HasFileSuffix(out var suffix))
            {
                var saveAsFileName = string.Concat(revitFile.Name, suffix);
                RevitFile = revitFile.ChangeFileName<RevitFile>(saveAsFileName);
            }
            RevitFile.Delete();
        }

        private string CreateSaveAsPath()
        {
            var saveAsPath = LibraryRootName;
            if (HasNewRootPath(out _))
            {
                saveAsPath = NewLibraryRootName;
            }
            if (HasBackupFolderName(out _))
            {
                if (IsAddFolderAtEnd())
                {
                    saveAsPath = string.Join(Constant.BackSlash, saveAsPath, LibraryPathName, SaveAsFolderName);
                }
                else
                {
                    saveAsPath = string.Join(Constant.BackSlash, saveAsPath, SaveAsFolderName, LibraryPathName);
                }
            }
            else
            {
                saveAsPath = string.Join(Constant.BackSlash, saveAsPath, LibraryPathName);
            }
            var revitFileName = RevitFileName;
            if (HasFileSuffix(out _))
            {
                revitFileName = string.Concat(revitFileName, Constant.Underline, SaveAsSuffixName);
            }
            saveAsPath = string.Join(Constant.BackSlash, saveAsPath, revitFileName);
            return saveAsPath;
        }

        private bool HasFileSuffix(out string suffix)
        {
            suffix = Parameters[FileSuffixIdx].Value;
            return string.IsNullOrEmpty(suffix) == false;
        }

        private bool HasBackupFolderName(out string folderName)
        {
            folderName = Parameters[BackupFolderIdx].Value;
            return string.IsNullOrEmpty(folderName) == false;
        }

        private bool IsAddFolderAtEnd()
        {
            if (bool.TryParse(Parameters[AddToEndIdx].Value, out var addToEnd))
            {
                return addToEnd;
            }
            return false;
        }

        private string GetRootPath()
        {
            var rootPath = Parameters[RootPathIdx].Value;
            rootPath = RemoveBackSlah(rootPath);
            return rootPath;
        }

        private bool HasNewRootPath(out string rootPath)
        {
            rootPath = Parameters[NewRootPathIdx].Value;
            rootPath = RemoveBackSlah(rootPath);
            return string.IsNullOrWhiteSpace(rootPath) == false;
        }

        private string RemoveBackSlah(string rootPath)
        {
            if (string.IsNullOrWhiteSpace(rootPath) == false
                && rootPath.EndsWith(Constant.BackSlash) == false)
            {
                rootPath = string.Concat(rootPath, Constant.BackSlash);
            }
            return rootPath;
        }
    }
}
