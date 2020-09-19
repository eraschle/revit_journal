using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitJournalUI.JournalTaskUI.Models
{
    public class PathViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected DirectoryViewModel Parent { get; }

        public PathViewModel(DirectoryViewModel parent)
        {
            Parent = parent;
        }

        private bool? _Checked = true;
        public bool? Checked
        {
            get { return _Checked; }
            set { SetChecked(value, true, true); }
        }

        internal void SetChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == _Checked) { return; }

            _Checked = value;
            if (updateChildren && Checked.HasValue)
            {
                UpdateChildren(_Checked.Value);
            }
            if (updateParent && Parent != null)
            {
                UpdateParent(_Checked);
            }
            OnPropertyChanged(nameof(Checked));
        }

        protected virtual void UpdateChildren(bool isChecked) { }

        protected virtual void UpdateParent(bool? isChecked)
        {
            Parent.UpdateCheckState();
            Parent.UpdateCheckedCount();
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
