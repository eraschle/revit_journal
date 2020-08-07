using DataSource.Helper;
using DataSource.Model.Catalog;
using DataSource.Model.FileSystem;
using DataSource.Model.Product;
using System;
using System.Collections.Generic;
using System.IO;

namespace DataSource
{
    public static class OmniClassManager
    {
        private const string AutodeskProgramDataRevitPrefix = @"RVT";
        private const string AutodeskProgramData = @"C:\ProgramData\Autodesk";
        private const string AutodeskProgramDataUserCache = @"UserDataCache";

        public static OmniClass Default { get; } = new OmniClass { IdArray = new int[] { 0 }, Name = Constant.Minus };

        private static readonly Dictionary<int, IList<OmniClass>> OmniClassVersions = new Dictionary<int, IList<OmniClass>>();

        public static IList<OmniClass> GetOmniClasses(RevitApp revitApp)
        {
            IList<OmniClass> omniClasses = new List<OmniClass>();
            if (OmniClassVersions.ContainsKey(revitApp.Version))
            {
                omniClasses = OmniClassVersions[revitApp.Version];
            }
            else if (HastOmniClassFile(revitApp, out var omniClassFile))
            {
                omniClasses = Create(omniClassFile);
                OmniClassVersions.Add(revitApp.Version, omniClasses);
            }
            else if (HasNewerVersion(revitApp, out var newerOmniClasses))
            {
                omniClasses = newerOmniClasses;
            }
            omniClasses.Insert(0, Default);
            return omniClasses;
        }

        private static bool HasNewerVersion(RevitApp revitApp, out IList<OmniClass> omniClasses)
        {
            omniClasses = null;
            int newVersion = revitApp.Version;
            foreach (int version in OmniClassVersions.Keys)
            {
                if (version <= revitApp.Version) { continue; }

                newVersion = Math.Min(newVersion, version);
            }
            if (newVersion > revitApp.Version)
            {
                omniClasses = OmniClassVersions[newVersion];
            }
            return omniClasses != null;
        }

        public static bool HastOmniClassFile(RevitApp revitApp, out OmniClassFile omniClass)
        {
            omniClass = null;
            if (revitApp is null) { return false; }

            if (HasAppFile(revitApp) == false && ProductManager.HasVersion(revitApp.Version, true))
            {
                ProductManager.UpdateVersions();
                revitApp = ProductManager.GetVersion(revitApp.Version, true);
            }

            if (HasAppFile(revitApp) == false || HasOmniClassDirectory(revitApp.AppFile, out var directory) == false) { return false; }

            var omniClassFile = revitApp.AppFile.ChangeExtension<OmniClassFile>(OmniClassFile.FileExtension)
                                                .ChangeFileName<OmniClassFile>(OmniClassFile.FileName)
                                                .ChangeDirectory<OmniClassFile>(directory);
            if (omniClassFile.Exist)
            {
                omniClass = omniClassFile;
            }
            return omniClass != null;
        }

        private static bool HasAppFile(RevitApp app)
        {
            return app.AppFile != null && app.AppFile.Exist;
        }

        public static bool HasOmniClassDirectory(RevitAppFile revitApp, out string directory)
        {
            directory = null;
            var userCacheFolder = string.Concat(AutodeskProgramDataRevitPrefix, Constant.Space, revitApp.Version);
            var userCache = Path.Combine(AutodeskProgramData, userCacheFolder, AutodeskProgramDataUserCache);
            if (Directory.Exists(userCache))
            {
                directory = userCache;
            }
            return directory != null;
        }

        private static IList<OmniClass> Create(OmniClassFile omniClassFile)
        {
            if (omniClassFile.Exist == false) { return null; }

            var content = File.ReadAllText(omniClassFile.FullPath);
            var lines = content.Split(new string[] { Environment.NewLine },
                                      StringSplitOptions.RemoveEmptyEntries);
            var omniClasses = new List<OmniClass>();
            foreach (var line in lines)
            {
                var values = line.Split(Constant.Tabulator);
                if (values.Length < 2) { continue; }

                var omniClass = new OmniClass
                {
                    IdArray = OmniClass.GetId(values[0]),
                    Name = values[1]
                };
                omniClasses.Add(omniClass);
            }
            return omniClasses;
        }
    }
}
