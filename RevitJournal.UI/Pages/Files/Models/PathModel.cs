using DataSource.Models.FileSystem;
using System;
using Utilities.System;
using Utilities.UI;

namespace RevitJournalUI.Pages.Files.Models
{
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

        internal virtual void SetChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == isChecked) { return; }

            isChecked = value;
            if (updateParent && Parent != null)
            {
                Parent.UpdateParent();
            }
            SetFileCount();
            UpdateChecked();
        }

        protected void SetFileCount()
        {
            var checkedCount = GetCheckedFilesCount(this);
            IsExpanded = IsExpanded && checkedCount > 0;
            FilesCountValue = $"{checkedCount} / {GetFilesCount(this)}";
        }

        private int GetFilesCount(PathModel model)
        {
            if (!(model is FolderModel folderModel)) { return 0; }

            var count = 0;
            foreach (var child in folderModel.Children)
            {
                if (child is FolderModel folder)
                {
                    count += GetFilesCount(folder);
                }
                else
                {
                    count += 1;
                }
            }
            return count;
        }

        private int GetCheckedFilesCount(PathModel model)
        {
            if (!(model is FolderModel folderModel)) { return 0; }

            var count = 0;
            foreach (var child in folderModel.Children)
            {
                if (child is FolderModel folder)
                {
                    count += GetCheckedFilesCount(folder);
                }
                else if (child.IsChecked == true)
                {
                    count += 1;
                }
            }
            return count;
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

        protected void UpdateChecked()
        {
            NotifyPropertyChanged(nameof(IsChecked));
        }
    }
}
