using DataSource.Models.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitJournal.Library
{
    public class LibraryFolder : ALibraryNode
    {
        public DirectoryNode Folder { get; private set; }

        public List<LibraryFolder> Subfolders { get; } = new List<LibraryFolder>();

        public List<LibraryFile> Files { get; } = new List<LibraryFile>();

        public List<LibraryFile> RecusiveFiles { get; } = new List<LibraryFile>();

        private readonly List<Action<object>> resetActions = new List<Action<object>>();
        private readonly List<Action<LibraryFile>> countActions = new List<Action<LibraryFile>>();

        public int FilteredCount { get; private set; } = 0;

        public int CheckedCount { get; private set; } = 0;

        public LibraryFolder(DirectoryNode directory, LibraryFolder parent) : base(parent)
        {
            Folder = directory ?? throw new ArgumentNullException(nameof(directory));
            Subfolders.AddRange(directory.GetDirectories<RevitFamilyFile>(true).Select(dir => new LibraryFolder(dir, this)));
            Files.AddRange(GetRevitFamilies(directory).Select(file => new LibraryFile(file, this)));

            AddCountAction(AllowedResetAction, AllowedCountAction);
            AddCountAction(CheckedAllowedResetAction, CheckedAllowedCountAction);
        }

        public void Setup()
        {
            RecusiveFiles.AddRange(Files);
            foreach (var folder in Subfolders)
            {
                folder.Setup();
                RecusiveFiles.AddRange(folder.RecusiveFiles);
            }
            UpdateFileCounts();
        }

        public static IEnumerable<RevitFamilyFile> GetRevitFamilies(DirectoryNode directory)
        {
            if (directory is null) { throw new ArgumentNullException(nameof(directory)); }

            var families = directory.GetFiles<RevitFamilyFile>(false)
                .Where(famFile => famFile.IsBackup() == false);
            return families;
        }

        public void UpdateFileCounts()
        {
            InvokeResetActions();
            foreach (var handler in RecusiveFiles)
            {
                InvokeCountActions(handler);
            }
        }

        private void InvokeResetActions()
        {
            foreach (var action in resetActions)
            {
                action.Invoke(null);
            }
        }

        private void InvokeCountActions(LibraryFile handler)
        {
            foreach (var action in countActions)
            {
                action.Invoke(handler);
            }
        }

        private void AllowedResetAction(object handler)
        {
            FilteredCount = 0;
        }

        private void AllowedCountAction(LibraryFile handler)
        {
            if (handler is null || handler.IsAllowed() == false) { return; }

            FilteredCount += 1;
        }

        private void CheckedAllowedResetAction(object handler)
        {
            CheckedCount = 0;
        }

        private void CheckedAllowedCountAction(LibraryFile handler)
        {
            if (handler is null || handler.IsCheckedAndAllowed() == false) { return; }

            CheckedCount += 1;
        }

        protected void AddCountAction(Action<object> resetAction, Action<LibraryFile> countAction)
        {
            resetActions.Add(resetAction);
            countActions.Add(countAction);
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

        public IEnumerable<RevitFamilyFile> CheckedFiles()
        {
            foreach (var handler in RecusiveFiles)
            {
                if (handler.IsCheckedAndAllowed() == false) { continue; }

                yield return handler.File;
            }
        }
    }
}
