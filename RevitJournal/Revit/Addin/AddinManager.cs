using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.RevitAddIns;
using DataSource.Model.FileSystem;
using RevitAction;

namespace RevitJournal.Revit.Addin
{
    public static class AddinManager
    {
        public const string VendorId = "RascerDev";

        public static void CreateAppManifest(DirectoryNode directory, ITaskAppInfo info)
        {
            if (info is null) { throw new ArgumentNullException(nameof(info)); }
            if (directory is null) { throw new ArgumentNullException(nameof(directory)); }

            if (string.IsNullOrEmpty(info.VendorId))
            {
                info.VendorId = VendorId;
            }
            var manifestPath = GetManifestPath(directory);
            if (File.Exists(manifestPath) == false)
            {
                File.Delete(manifestPath);
            }
            var manifest = new RevitAddInManifest();
            manifest.AddInApplications.Add(new RevitAddInApplication(info.ClassName,
                                                                     info.AssemblyPath,
                                                                     info.Id,
                                                                     info.FullClassName,
                                                                     info.VendorId));
            manifest.SaveAs(manifestPath);
        }

        public static void CreateManifest(DirectoryNode directory, ITaskInfo info)
        {
            if (info is null) { throw new ArgumentNullException(nameof(info)); }
            if (directory is null) { throw new ArgumentNullException(nameof(directory)); }

            var manifestPath = GetManifestPath(directory);
            var manifest = AddInManifestUtility.GetRevitAddInManifest(manifestPath);
            var commands = manifest.AddInCommands;
            if (HasCommand(commands, info.Id) == false)
            {
                if (string.IsNullOrEmpty(info.VendorId))
                {
                    info.VendorId = VendorId;
                }
                var newCommand = new RevitAddInCommand(info.AssemblyPath,
                                                       info.Id,
                                                       info.FullClassName,
                                                       info.VendorId);
                commands.Add(newCommand);
                manifest.Save();
            }
        }

        private static string GetManifestPath(DirectoryNode directory)
        {
            return Path.Combine(directory.FullPath, "RevitJournalAddins.addin");
        }

        private static bool HasCommand(IEnumerable<RevitAddInCommand> commands, Guid guid)
        {
            return commands.Any(cmd => cmd.AddInId.Equals(guid));
        }
    }
}
