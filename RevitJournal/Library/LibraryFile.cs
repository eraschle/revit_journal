using DataSource.Models.FileSystem;
using RevitJournal.Revit.Filtering;
using System;

namespace RevitJournal.Library
{
    public class LibraryFile : ALibraryNode
    {
        public RevitFamilyFile File { get; private set; }

        public LibraryFile(RevitFamilyFile revitFamily, LibraryFolder folder) : base(folder)
        {
            File = revitFamily ?? throw new ArgumentNullException(nameof(revitFamily));
        }

        public bool IsCheckedAndAllowed()
        {
            return IsChecked == true && IsAllowed();
        }

        public bool IsAllowed()
        {
            return RevitFilterManager.Instance.FileFilter(File);
        }

        internal override void SetChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if(value is null) { return; }

            base.SetChecked(value, updateChildren, updateParent);
        }
    }
}
