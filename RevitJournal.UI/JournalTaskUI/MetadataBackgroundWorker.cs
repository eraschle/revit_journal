using DataSource.Helper;
using RevitJournalUI.JournalTaskUI.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace RevitJournalUI.JournalTaskUI
{
    public static class MetadataBackgroundWorker
    {
        public static BackgroundWorker CreateWorker()
        {
            var worker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            worker.DoWork += new DoWorkEventHandler(OnDoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(OnProgressChanged);
            return worker;
        }

        public static void OnDoWork(object sender, DoWorkEventArgs args)
        {
            if (args is null || !(sender is BackgroundWorker worker)) { return; }
            if (!(args.Argument is IEnumerable<DirectoryViewModel> models)) { return; }
            if (worker.CancellationPending) { return; }

            foreach (var model in models)
            {
                UpdateMetadata(worker, model);
            }
            args.Result = args.Argument;
        }

        private static void OnProgressChanged(object sender, ProgressChangedEventArgs args)
        {
            if (args is null || !(args.UserState is FamilyViewModel viewModel)) { return; }

            var revitFamily = viewModel.RevitFamily;
            viewModel.MetadataStatus = revitFamily.MetadataStatus;
            var lastUpdate = DateHelper.AsString(revitFamily.Metadata.Updated);
            viewModel.LastUpdate = lastUpdate;
        }

        private static void UpdateMetadata(BackgroundWorker worker, DirectoryViewModel model)
        {
            var familyViewModels = model.RecursiveFamilyViewModel;
            var filesCount = familyViewModels.Count;
            var currentCount = 0;
            Parallel.For(0, filesCount, (idx) =>
            {
                if (worker.CancellationPending) { return; }

                var familyModel = familyViewModels[idx];
                familyModel.RevitFamily.UpdateStatus();
                currentCount++;
                var percent = currentCount * 100 / filesCount;
                worker.ReportProgress(percent, familyModel);
            });
        }

  

    }
}
