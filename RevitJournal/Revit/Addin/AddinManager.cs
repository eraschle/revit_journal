using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.RevitAddIns;
using RevitAction;
using RevitAction.Action;
using RevitJournal.Properties;

namespace RevitJournal.Revit.Addin
{
    public static class AddinManager
    {
        public static Guid DummyGuid { get; } = new Guid("5cd4edd6-51ef-46c3-9afe-3b9d5fced66e");

        public static Guid AppAddinId { get; } = new Guid("5aa93805-8e2a-4b8a-8087-94556cebe3b7");

        public const string VendorId = "RascerDev";

        public static void CreateAppManifest(string outputPath, ITaskAppInfo appInfo)
        {
            var manifest = GetManifest(outputPath);
            var addinApps = manifest.AddInApplications;
            var addinApp = addinApps.FirstOrDefault(app => app.AddInId == AppAddinId);
            addinApp.VendorId = appInfo.VendorId;
            addinApp.Assembly = appInfo.AssemblyPath;
            addinApp.FullClassName = appInfo.FullClassName;
            var commands = manifest.AddInCommands;
            if (HasCommand(commands, DummyGuid))
            {
                var dummyCmd = commands.First(cmd => IsCommand(cmd, DummyGuid));
                if (dummyCmd != null)
                {
                    commands.Remove(dummyCmd);
                    manifest.Save();
                }
            }
            manifest.Save();
        }

        public static void CreateManifest(string outputPath, ITaskActionCommand action)
        {
            if (action is null) { throw new ArgumentNullException(nameof(action)); }

            var manifest = GetManifest(outputPath);
            var commands = manifest.AddInCommands;
            if (HasCommand(commands, action.Id) == false)
            {
                var newCommand = new RevitAddInCommand(action.AssemblyPath,
                                                       action.Id,
                                                       action.FullClassName,
                                                       action.VendorId);
                commands.Add(newCommand);
                manifest.Save();
            }
        }

        private static RevitAddInManifest GetManifest(string outputPath)
        {
            var manifestPath = Path.Combine(outputPath, "RevitJournalAddins.addin");
            if (File.Exists(manifestPath) == false)
            {
                var template = Resources.AddinTemplate;
                File.WriteAllBytes(manifestPath, template);
            }
            return AddInManifestUtility.GetRevitAddInManifest(manifestPath);
        }

        private static bool HasCommand(IEnumerable<RevitAddInCommand> commands, Guid guid)
        {
            return commands.Any(cmd => IsCommand(cmd, guid));
        }


        private static bool IsCommand(RevitAddInCommand command, Guid guid)
        {
            return command.AddInId.Equals(guid);
        }
    }
}
