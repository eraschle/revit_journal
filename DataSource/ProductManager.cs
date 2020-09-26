using DataSource.Helper;
using DataSource.Model.FileSystem;
using DataSource.Model.Product;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataSource
{
    public static class ProductManager
    {
        private const string AutodeskProgram = @"C:\Program Files\Autodesk";

        public static RevitApp UseMetadata { get; } = RevitApp.DefaultApp;

        private static Dictionary<int, RevitApp> RevitVersions { get; } = new Dictionary<int, RevitApp>();

        public static void UpdateVersions(string rootDirectory = AutodeskProgram)
        {
            if (ExecutableRevitApps.Any()) { return; }

            var factory = PathFactory.Instance;
            var root = factory.CreateRoot(rootDirectory);
            var appFiles = factory.CreateFiles<RevitAppFile>(root, $"{RevitAppFile.RevitFileName}");
            foreach (var file in appFiles)
            {
                AddInstalled(file);
            }
        }

        public static ICollection<RevitApp> RevitApps
        {
            get { return RevitVersions.Values; }
        }

        public static IEnumerable<RevitApp> ExecutableRevitApps
        {
            get { return RevitApps.Where(rvt => rvt.Executable); }
        }

        public static RevitApp OldestVersion()
        {
            if (RevitApps.Count == 0)
            {
                UpdateVersions();
            }
            RevitApp oldest = null;
            foreach (var revitApp in ExecutableRevitApps)
            {
                if (oldest is null)
                {
                    oldest = revitApp;
                    continue;
                }
                if (revitApp.Version < oldest.Version)
                {
                    oldest = revitApp;
                }
            }
            return oldest;
        }

        private static void AddInstalled(RevitAppFile appFile)
        {
            const bool executable = true;
            if (appFile is null || HasVersion(appFile.Version, executable)) { return; }

            AddApp(new RevitApp(appFile));
        }

        public static void AddCustom(RevitApp app)
        {
            AddApp(app);
        }

        private static void AddApp(RevitApp app)
        {
            if (app is null) { return; }

            if (RevitVersions.ContainsKey(app.Version) && app.Executable)
            {
                RevitVersions[app.Version] = app;
            }
            else if (RevitVersions.ContainsKey(app.Version) == false)
            {
                RevitVersions.Add(app.Version, app);
            }
        }

        public static RevitApp CreateCustom(string name, int version)
        {
            return HasVersion(version, false)
                ? GetVersion(version, false)
                : new RevitApp(name, version);
        }

        public static bool HasVersion(int version, bool onlyExist)
        {
            if (RevitVersions.ContainsKey(version) == false) { return false; }

            if (onlyExist == false) { return true; }

            return RevitVersions[version].Executable == onlyExist;
        }

        public static RevitApp GetVersion(int version, bool onlyExist)
        {
            if (HasVersion(version, onlyExist) == false) { return null; }

            return RevitVersions[version];
        }

        public static bool HasVersionOrNewer(int version, bool onlyExist = true)
        {
            return HasVersion(version, onlyExist)
                || GetNewerRevitVersion(version, onlyExist) > version;
        }

        private static int GetNewerRevitVersion(int version, bool onlyExist)
        {
            var newerVersion = version;
            foreach (var revitVersion in RevitVersions.Keys)
            {
                var app = RevitVersions[version];
                if (revitVersion < version ||
                    onlyExist == true && app.Executable == false) { continue; }

                if (newerVersion == version)
                {
                    newerVersion = revitVersion;
                }
                newerVersion = Math.Min(newerVersion, revitVersion);
            }
            return newerVersion;
        }

        public static RevitApp GetVersionOrNewer(int version, bool onlyExist)
        {
            if (HasVersion(version, onlyExist)) { return GetVersion(version, onlyExist); }

            var newerVersion = GetNewerRevitVersion(version, onlyExist);
            return GetVersion(newerVersion, onlyExist);
        }
    }
}
