using DataSource.Model.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitJournal.Library
{
    public class SelectFolderHandler
    {
        public RevitDirectory Folder { get; private set; }

        public List<SelectFolderHandler> Subfolders { get; } = new List<SelectFolderHandler>();

        public List<SelectFileHandler> Files { get; } = new List<SelectFileHandler>();


        private List<SelectFileHandler> recusiveFiles = new List<SelectFileHandler>();
        public List<SelectFileHandler> RecusiveFiles { get { return recusiveFiles; } }

        private bool? isSelected = true;
        public bool? IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                UpdateFileHandlers(IsSelected);
            }
        }

        private void UpdateFileHandlers(bool? isChecked)
        {
            if (isChecked is null) { return; }

            var isSelcted = (bool)isChecked;
            foreach (var fileHandler in RecusiveFiles)
            {
                fileHandler.IsSelected = isSelcted;
            }
        }

        public SelectFolderHandler(RevitDirectory directory)
        {
            Folder = directory ?? throw new ArgumentNullException(nameof(directory));
            Subfolders.AddRange(directory.Subfolder.Select(dir => new SelectFolderHandler(dir)));
            Files.AddRange(directory.Files.Select(file => new SelectFileHandler(file, this)));
        }

        public void Setup()
        {
            AddRecursiveFiles(ref recusiveFiles);
            foreach (var folder in Subfolders)
            {
                folder.Setup();
            }
        }

        public int GetFilteredRecusiveCount(FilterManager manager, out int selected)
        {
            var filtered = 0;
            selected = 0;
            foreach (var handler in RecusiveFiles)
            {
                if (IsAllowed(handler, manager, false))
                {
                    filtered += 1;
                }
                if (IsAllowed(handler, manager, true))
                {
                    selected += 1;
                }
            }
            return filtered;
        }

        public IEnumerable<SelectFileHandler> SelectedRecusiveFiles(FilterManager manager)
        {
            foreach (var handler in recusiveFiles)
            {
                if (IsAllowed(handler, manager, true) == false) { continue; }

                yield return handler;
            }
        }

        private bool IsAllowed(SelectFileHandler fileHandler, FilterManager manager, bool selected)
        {
            return selected == false
                ? IsFileFilter(fileHandler, manager)
                : fileHandler.IsSelected && IsFileFilter(fileHandler, manager);
        }

        private bool IsFileFilter(SelectFileHandler fileHandler, FilterManager manager)
        {
            return manager is object && manager.FileFilter(fileHandler.File);
        }

        private void AddRecursiveFiles(ref List<SelectFileHandler> revitFamilies)
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
