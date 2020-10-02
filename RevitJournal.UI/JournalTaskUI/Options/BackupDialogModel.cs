using DataSource.Model.FileSystem;
using RevitJournal.Helper;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Utilities.System;
using Utilities.UI;

namespace RevitJournalUI.JournalTaskUI.Options
{
    public class BackupDialogModel : ANotifyPropertyChangedModel
    {
        private const string SelectDialogTitle = "Choose New Root Path";

        internal PathCreator creator;

        public BackupDialogModel()
        {
            SelectCommand = new RelayCommand<object>(SelectCommandAction);
            ClearCommand = new RelayCommand<object>(ClearCommandAction);
            DefaultCommand = new RelayCommand<TextBox>(DefaultCommandAction);
        }

        public void Update(PathCreator pathCreator)
        {
            if (pathCreator is null) { throw new ArgumentNullException(nameof(pathCreator)); }

            creator = pathCreator;
            RootPath = pathCreator.RootPath;
            NewRootPath = pathCreator.NewRootPath;
            BackupFolder = pathCreator.BackupFolder;
            AddBackupAtEnd = pathCreator.AddBackupAtEnd;
            FileSuffix = pathCreator.FileSuffix;
        }

        public PathCreator GetPathCreator()
        {
            return creator;
        }

        public string RootPath
        {
            get { return creator is null ? string.Empty : creator.RootPath; }
            set
            {
                creator.SetRoot(value);
                NotifyPropertyChanged();
            }
        }

        public string NewRootPath
        {
            get { return creator is null ? string.Empty : creator.NewRootPath; }
            set
            {
                if (StringUtils.Equals(creator.NewRootPath, value)) { return; }

                creator.SetNewRoot(value);
                NotifyPropertyChanged();
            }
        }

        public bool AddBackupAtEnd
        {
            get { return creator is null ? false : creator.AddBackupAtEnd; }
            set
            {
                if (creator.AddBackupAtEnd == value) { return; }

                creator.AddBackupAtEnd = value;
                NotifyPropertyChanged();
            }
        }

        public string BackupFolder
        {
            get { return creator is null ? string.Empty : creator.BackupFolder; }
            set
            {
                if (StringUtils.Equals(creator.BackupFolder, value)) { return; }

                creator.BackupFolder = value;
                NotifyPropertyChanged();
            }
        }

        public string FileSuffix
        {
            get { return creator is null ? string.Empty : creator.FileSuffix; }
            set
            {
                if (StringUtils.Equals(creator.FileSuffix, value)) { return; }

                creator.FileSuffix = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand SelectCommand { get; }

        private void SelectCommandAction(object parameter)
        {
            var selectedPath = creator.NewRootPath;
            if (string.IsNullOrWhiteSpace(creator.NewRootPath))
            {
                selectedPath = creator.RootPath;
            }
            NewRootPath = PathDialog.ChooseDir(SelectDialogTitle, selectedPath);
        }

        public ICommand ClearCommand { get; }

        private void ClearCommandAction(object parameter)
        {
            if (parameter is TextBlock block)
            {
                block.Text = string.Empty;
                UpdateSource(block);
            }
            else if (parameter is TextBox box)
            {
                box.Text = string.Empty;
                UpdateSource(box);
            }
        }

        public ICommand DefaultCommand { get; }

        private void DefaultCommandAction(TextBox box)
        {
            box.Text = DateUtils.GetPathDate();
            UpdateSource(box);
        }

        private static void UpdateSource(TextBlock block)
        {
            if (block is null) { return; }

            var binding = block.GetBindingExpression(TextBlock.TextProperty);
            if (binding is null) { return; }

            binding.UpdateSource();
        }

        private static void UpdateSource(TextBox box)
        {
            if (box is null) { return; }

            var binding = box.GetBindingExpression(TextBox.TextProperty);
            if (binding is null) { return; }

            binding.UpdateSource();
        }
    }
}
