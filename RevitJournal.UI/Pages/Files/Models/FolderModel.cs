using DataSource.Models.FileSystem;
using System.Collections.ObjectModel;
using Utilities.System;

namespace RevitJournalUI.Pages.Files.Models
{
    public class FolderModel : PathModel
    {
        public FolderModel(DirectoryNode pathNode) : base(pathNode) { }

        public ObservableCollection<PathModel> Children { get; }
            = new ObservableCollection<PathModel>();

        internal override void SetChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == isChecked) { return; }

            isChecked = value;
            if (updateChildren && isChecked.HasValue)
            {
                foreach (var node in Children)
                {
                    node.SetChecked(isChecked, true, false);
                }
            }
            if (updateParent && Parent != null)
            {
                Parent.UpdateParent();
            }
            UpdateChecked();
        }

        private string filesCountValue = "10/40";
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

        public void UpdateParent()
        {
            bool? state = null;
            for (int i = 0; i < Children.Count; ++i)
            {
                bool? current = Children[i].IsChecked;
                if (i == 0)
                {
                    state = current;
                }
                else if (state != current)
                {
                    state = null;
                    break;
                }
            }
            SetChecked(state, false, true);
        }
    }
}
