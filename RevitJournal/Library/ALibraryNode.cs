using DataSource.Model.FileSystem;
using System;

namespace RevitJournal.Library
{
    public abstract class ALibraryNode
    {
        protected LibraryFolder Parent { get; private set; }

        public ALibraryNode(LibraryFolder folder)
        {
            Parent = folder;
        }

        private bool? isChecked = true;
        public bool? IsChecked
        {
            get { return isChecked; }
            set { SetChecked(value, true, true); }
        }

        internal virtual void SetChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == isChecked) { return; }

            isChecked = value;
            if (updateChildren)
            {
                UpdateChildren();
            }
            if (updateParent && Parent != null)
            {
                UpdateParent();
            }
        }

        protected virtual void UpdateParent()
        {
            Parent.UpdateCheckedStatus();
            Parent.UpdateFileCounts();
        }

        protected virtual void UpdateChildren()
        {

        }
    }
}
