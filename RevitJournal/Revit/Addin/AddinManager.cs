using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.RevitAddIns;
using RevitAction.Action;
using RevitJournal.Properties;

namespace RevitJournal.Revit.Addin
{
    public static class AddinManager
    {
        public static Guid DummyGuid
        {
            get { return new Guid("5cd4edd6-51ef-46c3-9afe-3b9d5fced66e"); }
        }

        public static void CreateManifest(string outputPath, ITaskActionCommand action)
        {
            if (action is null) { throw new ArgumentNullException(nameof(action)); }

            var manifest = GetManifest(outputPath);
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
            if (HasCommand(commands, action.AddinId) == false)
            {
                var newCommand = new RevitAddInCommand(action.AssemblyPath,
                                                       action.AddinId,
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
