using DataSource.Model.Product;
using System;

namespace RevitJournal.Revit
{
    public class RevitArguments
    {
        public RevitApp RevitApp { get; set; }

        public bool ShowSplash
        {
            get { return RevitApp.ShowSlash; }
        }

        public string ShowSplashArgument { get; } = "/nosplash ";

        public string Language
        {
            get { return RevitApp.Language; }
        }

        public string LanguageArgument { get; } = "/language ";

        public bool StartMinimized
        {
            get { return RevitApp.StartMinimized; }
        }

        public string StartMinimizedArgument { get; } = "/min ";

        public string RevitExecutable
        {
            get { return RevitApp.AppFile.FullPath; }
        }

        public string WorkingDirectory
        {
            get { return RevitApp.AppFile.ParentFolder; }
        }

        public static TimeSpan MinimumTimeout { get { return TimeSpan.FromMinutes(1); } }

        public static TimeSpan DefaultTimeout { get { return TimeSpan.FromMinutes(2); } }

        public static TimeSpan MaximumTimeout { get { return TimeSpan.FromMinutes(20); } }

        public TimeSpan Timeout { get; set; } = DefaultTimeout;
    }
}
