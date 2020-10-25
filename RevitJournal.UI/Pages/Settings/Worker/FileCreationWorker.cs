using DataSource.Models.FileSystem;
using RevitJournal.Revit.Filtering;
using RevitJournal.Tasks.Options;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace RevitJournalUI.Pages.Settings.Worker
{
    public static class FileCreationWorker
    {
        private static TaskOptions options;


        public static BackgroundWorker Create(TaskOptions taskOptions)
        {
            options = taskOptions ?? throw new ArgumentNullException(nameof(taskOptions));
            var worker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            worker.DoWork += OnDoWork;
            worker.ProgressChanged += OnProgressChanged;
            worker.RunWorkerCompleted += OnRunWorkerCompleted;
            return worker;
        }

        public static void OnDoWork(object sender, DoWorkEventArgs args)
        {
            if(args is null) { throw new ArgumentNullException(nameof(args)); }
            if (!(sender is BackgroundWorker worker)) { return; }
            if (worker.CancellationPending) { return; }

            StatusBarViewModel.Instance.SetPercentage("Create files");
            var root = options.GetFamilyRoot();
            var files = root.GetFilePaths<RevitFamilyFile>();
            var currentCount = 0;
            for (int idx = 0; idx < files.Count; idx++)
            {
                if (worker.CancellationPending)
                {
                    args.Cancel = true;
                    return;
                }

                var fileNode = root.CreateFile<RevitFamilyFile>(files[idx]);
                currentCount++;
                var percent = currentCount * 100 / files.Count;
                worker.ReportProgress(percent, fileNode);
            }
            args.Result = args.Argument;
        }

        private static void OnProgressChanged(object sender, ProgressChangedEventArgs args)
        {
            if (args is null || !(args.UserState is RevitFamilyFile family)) { return; }

            StatusBarViewModel.Instance.ProgressText = family.NameWithoutExtension;
            StatusBarViewModel.Instance.ProgressValue = args.ProgressPercentage;
        }

        private static void OnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StatusBarViewModel.Instance.Reset();
        }
    }
}
