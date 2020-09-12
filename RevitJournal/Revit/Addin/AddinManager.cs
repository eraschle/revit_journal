using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.RevitAddIns;
using RevitAction;

namespace RevitJournal.Revit.Addin
{
    public static class AddinManager
    {
        public const string VendorId = "RascerDev";

        public static void CreateAppManifest(string outputPath, ITaskAppInfo info)
        {
            if (info is null) { throw new ArgumentNullException(nameof(info)); }

            if (string.IsNullOrEmpty(info.VendorId))
            {
                info.VendorId = VendorId;
            }
            var manifestPath = GetManifestPath(outputPath);
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

        public static void CreateManifest(string outputPath, ITaskInfo info)
        {
            if (info is null) { throw new ArgumentNullException(nameof(info)); }

            var manifestPath = GetManifestPath(outputPath);
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

        private static string GetManifestPath(string outputPath)
        {
            return Path.Combine(outputPath, "RevitJournalAddins.addin");
        }

        private static bool HasCommand(IEnumerable<RevitAddInCommand> commands, Guid guid)
        {
            return commands.Any(cmd => cmd.AddInId.Equals(guid));
        }
    }
}
