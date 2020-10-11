using DataSource.Models.FileSystem;
using RevitJournal.Revit.Filtering;
using RevitJournal.Tasks.Options.Parameter;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace RevitJournalUI.Pages.Settings.Worker
{
    public static class MetadataWorker
    {
        public static BackgroundWorker Create()
        {
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

        public static async void OnDoWork(object sender, DoWorkEventArgs args)
        {
            if (args is null || !(args.Argument is ITaskOptionDirectory optionDirectory)) { return; }
            if (!(sender is BackgroundWorker worker)) { return; }
            if (worker.CancellationPending) { return; }

            StatusBarViewModel.Instance.SetPercentage("Loading Metadata");
            var rootNode = optionDirectory.GetRootNode<RevitFamilyFile>();
            var files = rootNode.GetFiles<RevitFamilyFile>(true);
            var currentCount = 0;
            using (var cancel = new CancellationTokenSource())
            {

                var options = new ParallelOptions { CancellationToken = cancel.Token };
                Parallel.For(0, files.Count, options, (idx) =>
                {
                    if (worker.CancellationPending)
                    {
                        cancel.Cancel();
                        args.Cancel = true;
                        return;
                    }

                    var file = files[idx];
                    file.Update();
                    currentCount++;
                    var percent = currentCount * 100 / files.Count;
                    worker.ReportProgress(percent, file);
                });
            }
            args.Result = args.Argument;
        }

        private static void OnProgressChanged(object sender, ProgressChangedEventArgs args)
        {
            if (args is null || !(args.UserState is RevitFamilyFile family)) { return; }

            RevitFilterManager.Instance.AddValue(family);
            StatusBarViewModel.Instance.ProgressText = family.NameWithoutExtension;
            StatusBarViewModel.Instance.ProgressValue = args.ProgressPercentage;
        }

        private static void OnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StatusBarViewModel.Instance.Reset();
        }
    }
}
