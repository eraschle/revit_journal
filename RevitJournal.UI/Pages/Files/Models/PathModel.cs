using DataSource.Models.FileSystem;
using System;
using System.Collections.ObjectModel;
using Utilities.UI;

namespace RevitJournalUI.Pages.Files.Models
{
    public abstract class PathModel<TPathNode> : ANotifyPropertyChangedModel where TPathNode : APathNode
    {
        public TPathNode PathNode { get; private set; }

        protected FolderModel Parent { get; }

        protected PathModel(TPathNode pathNode, FolderModel parent)
        {
            PathNode = pathNode ?? throw new ArgumentNullException(nameof(pathNode));
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        public ObservableCollection<PathModel<APathNode>> Children { get; }
            = new ObservableCollection<PathModel<APathNode>>();

        public string Name
        {
            get { return PathNode.Name; }
        }

        private bool? isChecked = true;
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
                foreach (var node in Children)
                {
                    node.SetChecked(isChecked, true, false);
                }
            }
            if (updateParent && Parent != null)
            {
                UpdateParent();
            }
            UpdateChecked();
        }

        protected void UpdateChecked()
        {
            NotifyPropertyChanged(nameof(IsChecked));
        }

        protected void UpdateParent()
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
