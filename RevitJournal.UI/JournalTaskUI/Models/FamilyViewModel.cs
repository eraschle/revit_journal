﻿using RevitJournalUI.MetadataUI;
using System.Windows.Input;
using RevitJournal.Library;
using System;
using DataSource.Model;
using Utilities.System;
using Utilities.UI;

namespace RevitJournalUI.JournalTaskUI.Models
{
    public class FamilyViewModel : PathViewModel<LibraryFile>
    {
        public FamilyViewModel(LibraryFile fileHandler, DirectoryViewModel parent) : base(fileHandler, parent)
        {
            ViewMetadataCommand = new RelayCommand<object>(ViewMetadataCommandAction);
        }

        public void AddMetadataEvent()
        {
            Handler.File.MetadataUpdated += File_MetadataUpdated;
        }

        public void RemoveMetadataEvent()
        {
            Handler.File.MetadataUpdated -= File_MetadataUpdated;
        }

        private void File_MetadataUpdated(object sender, EventArgs args)
        {
            UpdateMetadata();
        }

        public MetadataStatus MetadataStatus
        {
            get { return Handler.File.MetadataStatus; }
        }

        public string RevitFileName
        {
            get { return Handler.File.RevitFile.NameWithoutExtension; }
        }

        private bool enabled = true;
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled == value) { return; }

                enabled = value;
                NotifyPropertyChanged();
            }
        }

        public string LastUpdate
        {
            get
            {
                var metadata = Handler.File.Metadata;
                return metadata is object
                    ? DateUtils.AsString(metadata.Updated)
                    : string.Empty;
            }
        }

        private void UpdateMetadata()
        {
            NotifyPropertyChanged(nameof(MetadataStatus));
            NotifyPropertyChanged(nameof(LastUpdate));
        }

        public ICommand ViewMetadataCommand { get; }

        private void ViewMetadataCommandAction(object parameter)
        {
            var dialog = new MetadataDialogView(Handler.File.Metadata);
            dialog.ShowDialog();
        }


    }
}
