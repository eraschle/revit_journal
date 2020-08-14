using System;
using System.IO;
using Autodesk.RevitAddIns;
using RevitAction.Action;
using RevitCommand.Families;
using RevitJournal.Properties;

namespace RevitJournal.Revit.Commands
{
    public static class FileCreationManager
    {
        public const string AddinFileName = "RevitJournalAddins.addin";
        public static Guid DummyAddinGuid { get; } = new Guid("5cd4edd6-51ef-46c3-9afe-3b9d5fced66e");

        public static bool AddinFileExiste(string outputPath)
        {
            return File.Exists(GetAddinManifestPath(outputPath));
        }

        public static void CreateAddinFile(string outputPath, ITaskActionCommand action)
        {
            var manifest = GetCurrentManifest(outputPath);
            if (HasCommand(manifest, DummyAddinGuid))
            {
                RevitAddInCommand dummyCommand = null;
                foreach (var command in manifest.AddInCommands)
                {
                    if (command.AddInId.Equals(DummyAddinGuid))
                    {
                        dummyCommand = command;
                        break;
                    }
                }
                if (dummyCommand != null)
                {
                    manifest.AddInCommands.Remove(dummyCommand);
                    manifest.Save();
                }
            }
            if (HasCommand(manifest, action.AddinId) == false)
            {
                var newCommand = CreateNewAddinCommand(action);
                manifest.AddInCommands.Add(newCommand);
                manifest.Save();
            }
        }

        private static void CreateManifest(string outputPath)
        {
            var template = Resources.AddinTemplate;
            var addinPath = GetAddinManifestPath(outputPath);
            File.WriteAllBytes(addinPath, template);
        }

        private static string GetAddinManifestPath(string outputPath)
        {
            return Path.Combine(outputPath, AddinFileName);
        }

        private static RevitAddInManifest GetCurrentManifest(string outputPath)
        {
            if (AddinFileExiste(outputPath) == false)
            {
                CreateManifest(outputPath);
            }

            var addinPath = GetAddinManifestPath(outputPath);
            return AddInManifestUtility.GetRevitAddInManifest(addinPath);
        }

        private static RevitAddInCommand CreateNewAddinCommand(ITaskActionCommand action)
        {
            var newCommand = new RevitAddInCommand(
                AssemblyFilePath(), action.AddinId, action.FullClassName, action.VendorId);

            return newCommand;

        }

        private static string AssemblyFilePath()
        {
            var thisAssembly = typeof(FileCreationManager).Assembly.Location;
            return Path.Combine(Path.GetDirectoryName(thisAssembly), "RevitCommand.dll");
        }

        private static bool HasCommand(RevitAddInManifest manifest, Guid addinGuid)
        {
            var commandExist = false;
            foreach (var cmd in manifest.AddInCommands)
            {
                commandExist |= cmd.AddInId.Equals(addinGuid);

                if (commandExist) { break; }
            }
            return commandExist;
        }
    }
}
