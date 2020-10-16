using DataSource.Models.FileSystem;
using System;
using System.Collections.ObjectModel;
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

        protected bool? isChecked { get; set; } = true;
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
            UpdateChecked();
        }

        protected void UpdateChecked()
        {
            NotifyPropertyChanged(nameof(IsChecked));
        }
    }
}
