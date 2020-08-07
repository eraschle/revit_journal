using System;
using System.Collections.Generic;

namespace RevitJournalUI.Models
{
    public class CheckedDisplayViewModel : ACheckedViewModel, IEquatable<CheckedDisplayViewModel>
    {
        private string _DisplayName = string.Empty;
        public string DisplayName
        {
            get { return _DisplayName; }
            set
            {
                if (_DisplayName.Equals(value, StringComparison.CurrentCulture)) { return; }

                _DisplayName = value;
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        private string _GroupName = string.Empty;
        public string GroupName
        {
            get { return _GroupName; }
            set
            {
                if (_GroupName.Equals(value, StringComparison.CurrentCulture)) { return; }

                _GroupName = value;
                OnPropertyChanged(nameof(GroupName));
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CheckedDisplayViewModel);
        }

        public bool Equals(CheckedDisplayViewModel other)
        {
            return other != null &&
                   DisplayName == other.DisplayName &&
                   GroupName == other.GroupName;
        }

        public override int GetHashCode()
        {
            var hashCode = -464718739;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DisplayName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(GroupName);
            return hashCode;
        }
    }
}
