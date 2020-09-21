using RevitJournal.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitJournalUI.JournalTaskUI.Models
{
    public class PathViewModel<THandler> : INotifyPropertyChanged where THandler : ALibraryNode
    {
        public event PropertyChangedEventHandler PropertyChanged;

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
            OnPropertyChanged(nameof(Checked));
        }

        protected virtual void FinishSetChecked() { }

        protected virtual void UpdateChildren() { }

        protected virtual void UpdateParent()
        {
            var updateParent = true;
            Parent.UpdateCheckedCount(updateParent);
            Parent.UpdateCheckedStatus(updateParent);
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
