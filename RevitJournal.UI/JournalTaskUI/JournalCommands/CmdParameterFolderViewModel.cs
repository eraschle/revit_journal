using RevitJournalUI.Helper;
using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows;

namespace RevitJournalUI.JournalTaskUI.JournalCommands
{
    public class CmdParameterFolderViewModel : CmdParameterStringViewModel
    {
        private const string SelectFileTitle = "Select A Folder";

        public CmdParameterFolderViewModel() : base()
        {
            ChooseFolderCommand = new RelayCommand<string>(ChooseFolderCommandAction, ChooseCommandFilePredicate);
        }

        public ICommand ChooseFolderCommand{ get; private set; }

        private bool ChooseCommandFilePredicate(string parameter)
        {
            return true;
        }

        private void ChooseFolderCommandAction(string parameter)
        {
            ParameterValue = ChooseDirectory(parameter);
        }

        private static string ChooseDirectory(string selectedPath)
        {
            if(string.IsNullOrWhiteSpace(selectedPath) || Directory.Exists(selectedPath) == false)
            {
                selectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            using (var openDialog = new FolderBrowserDialog())
            {
                openDialog.Description = SelectFileTitle;
                openDialog.SelectedPath = selectedPath;
                openDialog.ShowNewFolderButton = false;
                var result = openDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    return openDialog.SelectedPath;
                }
            }
            return selectedPath;
        }
    }
}
