using DataSource.Models.FileSystem;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RevitJournalUI.Pages.Files.Models
{
    public class FolderModel : PathModel
    {
        public FolderModel(DirectoryNode pathNode) : base(pathNode) { }

        public ObservableCollection<PathModel> Children { get; }
            = new ObservableCollection<PathModel>();

        public IEnumerable<FolderModel> ChildFolders
        {
            get { return Children.OfType<FolderModel>(); }
        }

        protected override void UpdateChildren()
        {
            foreach (var node in Children)
            {
                node.SetChecked(isChecked, true, false);
            }
        }

        public void AddChild(PathModel pathModel)
        {
            if(pathModel is null || Children.Contains(pathModel)) { return; }

            pathModel.Parent = this;
            Children.Add(pathModel);
        }

        internal void UpdateParent()
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
