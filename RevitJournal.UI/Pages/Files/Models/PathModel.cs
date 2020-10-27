using DataSource.Models.FileSystem;
using System;
using System.Diagnostics;
using Utilities.System;
using Utilities.UI;

namespace RevitJournalUI.Pages.Files.Models
{
    [DebuggerDisplay("Name: {Name} Checked: {IsChecked}")]
    public abstract class PathModel : ANotifyPropertyChangedModel
    {
        public APathNode PathNode { get; private set; }

        public FolderModel Parent { get; set; }

        protected PathModel(APathNode pathNode)
        {
            PathNode = pathNode ?? throw new ArgumentNullException(nameof(pathNode));
        }

        public string Name
        {
            get { return PathNode.Name; }
        }

        protected bool? isChecked { get; set; } = false;
        public bool? IsChecked
        {
            get { return isChecked; }
            set { SetChecked(value, true, true); }
        }

        internal void SetChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == isChecked) { return; }

            isChecked = value;
            if (updateChildren && isChecked.HasValue)
            {
                UpdateChildren();
                CalculateFilesCount();
            }
            if (updateParent)
            {
                UpdateParent();
            }
            NotifyPropertyChanged(nameof(IsChecked));
        }

        internal void UpdateParent()
        {
            if(Parent is null) { return; }

            Parent.UpdateParent();
            Parent.CalculateFilesCount();
        }

        protected abstract void UpdateChildren();

        internal void CalculateFilesCount()
        {
            if (!(this is FolderModel folderModel)) { return; }

            var filesCount = 0;
            var validCount = 0;
            var checkedCount = 0;

            foreach (var child in folderModel.Children)
            {
                if (child is FolderModel folder)
                {
                    folder.CalculateFilesCount();
                    filesCount += folder.FileCount;
                    validCount += folder.ValidFileCount;
                    checkedCount += folder.CheckedFileCount;
                }
                else if (child is FileModel file)
                {
                    filesCount += file.FileCount;
                    checkedCount += file.CheckedFileCount;
                    validCount += file.ValidFileCount;
                }
            }
            FileCount = filesCount;
            ValidFileCount = validCount;
            CheckedFileCount = checkedCount;
            FilesCountValue = $"{CheckedFileCount} / {FileCount}";
        }

        internal virtual int FileCount { get; set; }

        private int validFilesCount = 0;
        internal virtual int ValidFileCount
        {
            get { return validFilesCount; }
            set
            {
                if (validFilesCount == value) { return; }

                validFilesCount = value;
                NotifyPropertyChanged();
            }
        }


        private int checkedFilesCount = 0;
        internal virtual int CheckedFileCount
        {
            get { return checkedFilesCount; }
            set
            {
                if (checkedFilesCount == value) { return; }

                checkedFilesCount = value;
                NotifyPropertyChanged();
            }
        }

        private string filesCountValue = string.Empty;
        public string FilesCountValue
        {
            get { return filesCountValue; }
            set
            {
                if (StringUtils.Equals(filesCountValue, value)) { return; }

                filesCountValue = value;
                NotifyPropertyChanged();
            }
        }

        private bool isExpanded = false;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                if (isExpanded == value) { return; }

                isExpanded = value;
                NotifyPropertyChanged();
            }
        }
    }
}
