using RevitJournalUI.Helper;
using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;

namespace RevitJournalUI.JournalTaskUI.JournalCommands
{
    public class CmdParameterFileViewModel : CmdParameterStringViewModel
    {
        private readonly string Title;
        private readonly string FileFilter;
        private readonly int FilterIndex;

        public CmdParameterFileViewModel(string title, string fileFilter, int filterIndex) : base()
        {
            Title = title;
            FileFilter = fileFilter;
            FilterIndex = filterIndex;
            ChooseCommandFile = new RelayCommand<string>(ChooseCommandFileAction, ChooseCommandFilePredicate);
        }

        public ICommand ChooseCommandFile { get; private set; }

        private bool ChooseCommandFilePredicate(string parameter)
        {
            return true;
        }

        private void ChooseCommandFileAction(string parameter)
        {
            ParameterValue = ChooseDirectory(parameter);
        }

        private string ChooseDirectory(string selectedPath)
        {
            using (var openDialog = new OpenFileDialog())
            {
                openDialog.Title = Title;
                openDialog.Filter = FileFilter;
                openDialog.FilterIndex = FilterIndex;
                openDialog.CheckFileExists = true;
                openDialog.Multiselect = false;
                openDialog.InitialDirectory = GetDirectory(selectedPath);
                var result = openDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    return openDialog.FileName;
                }
            }
            return selectedPath;
        }

        private static string GetDirectory(string preSelected)
        {
            if (string.IsNullOrWhiteSpace(preSelected))
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            else
            {
                return Path.GetDirectoryName(preSelected);
            }
        }
    }

}
