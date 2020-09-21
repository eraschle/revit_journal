using DataSource.Model.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitJournal.Library
{
    public class LibraryFolder : ALibraryNode
    {
        public RevitDirectory Folder { get; private set; }

        public List<LibraryFolder> Subfolders { get; } = new List<LibraryFolder>();

        public List<LibraryFile> Files { get; } = new List<LibraryFile>();

        private List<LibraryFile> recusiveFiles = new List<LibraryFile>();
        public List<LibraryFile> RecusiveFiles { get { return recusiveFiles; } }

        public int FilteredCount { get; private set; } = 0;

        public int CheckedCount { get; private set; } = 0;

        public LibraryFolder(RevitDirectory directory, LibraryFolder parent) : base(parent)
        {
            Folder = directory ?? throw new ArgumentNullException(nameof(directory));
            Subfolders.AddRange(directory.Subfolder.Select(dir => new LibraryFolder(dir, this)));
            Files.AddRange(directory.Files.Select(file => new LibraryFile(file, this)));
        }

        public void Setup()
        {
            AddRecursiveFiles(ref recusiveFiles);
            foreach (var folder in Subfolders)
            {
                folder.Setup();
            }
            UpdateFileCounts();
        }

        public virtual void UpdateFileCounts()
        {
            FilteredCount = 0;
            CheckedCount = 0;
            foreach (var handler in RecusiveFiles)
            {
                if (handler.IsAllowed())
                {
                    FilteredCount += 1;
                }
                if (handler.IsCheckedAndAllowed())
                {
                    CheckedCount += 1;
                }
                UpdateCountAction(handler);
            }
        }



        protected override void UpdateChildren()
        {
            base.UpdateChildren();
            if (IsChecked.HasValue == false) { return; }

            foreach (var file in Files)
            {
                file.SetChecked(IsChecked, true, false);
            }
            foreach (var folder in Subfolders)
            {
                folder.SetChecked(IsChecked, true, false);
            }

            UpdateFileCounts();
        }

        protected virtual void UpdateCountAction(LibraryFile handler) { }

        internal void UpdateCheckedStatus()
        {
            bool? state = null;
            for (int idx = 0; idx < RecusiveFiles.Count; ++idx)
            {
                bool? current = RecusiveFiles[idx].IsChecked;
                if (idx == 0)
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

        public IEnumerable<RevitFamily> CheckedFiles()
        {
            foreach (var handler in recusiveFiles)
            {
                if (handler.IsCheckedAndAllowed() == false) { continue; }

                yield return handler.File;
            }
        }

        private void AddRecursiveFiles(ref List<LibraryFile> revitFamilies)
        {
            if (revitFamilies is null) { throw new ArgumentNullException(nameof(revitFamilies)); }

            revitFamilies.AddRange(Files);
            foreach (var folder in Subfolders)
            {
                folder.AddRecursiveFiles(ref revitFamilies);
            }
        }
    }
}
