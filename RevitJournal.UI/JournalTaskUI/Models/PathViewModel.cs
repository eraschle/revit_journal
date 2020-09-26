using RevitJournal.Library;
using System;
using Utilities.UI;

namespace RevitJournalUI.JournalTaskUI.Models
{
    public class PathViewModel<THandler> : ANotifyPropertyChangedModel where THandler : ALibraryNode
    {
        public THandler Handler { get; private set; }

        protected DirectoryViewModel Parent { get; }

        protected PathViewModel(THandler handler, DirectoryViewModel parent)
        {
            Handler = handler ?? throw new ArgumentNullException(nameof(parent));
            Parent = parent;
        }

        public bool? Checked
        {
            get { return Handler.IsChecked; }
            set { SetChecked(value, true, true); }
        }

        internal void SetChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == Handler.IsChecked) { return; }

            Handler.IsChecked = value;
            if (updateChildren)
            {
                UpdateChildren();
            }
            if (updateParent && Parent != null)
            {
                UpdateParent();
            }
            FinishSetChecked();
            UpdateChecked();
        }

        internal void UpdateChecked()
        {
            NotifyPropertyChanged(nameof(Checked));
        }

        protected virtual void FinishSetChecked() { }

        protected virtual void UpdateChildren() { }

        protected virtual void UpdateParent()
        {
            var updateParent = true;
            Parent.UpdateCheckedCount(updateParent);
            Parent.UpdateCheckedStatus(updateParent);
        }
    }
}
