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
            get
            {
                return RevitApp.AppFile.HasParent(out var parent) == false
                    ? string.Empty : parent.FullPath;
            }
        }

        public TimeSpan Timeout { get; set; }

        public int TimeoutTime
        {
            get { return (int)Timeout.TotalMilliseconds; }
        }
    }
}
