using RevitJournalUI.Helper;
using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;

namespace RevitJournalUI.JournalTaskUI.Parameters
{
    public class JournalCommandParameterFileViewModel : JournalCommandParameterStringViewModel
    {
        private const string SelectFileTitle = "Select A File";
        private const string SelectFileFilter = "Text Files (*.txt)|*.txt";

        public JournalCommandParameterFileViewModel() : base()
        {
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

        private static string ChooseDirectory(string selectedPath)
        {
            using (var openDialog = new OpenFileDialog())
            {
                openDialog.Title = SelectFileTitle;
                openDialog.Filter = SelectFileFilter;
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
