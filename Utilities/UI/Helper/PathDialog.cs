using System.IO;
using System.Windows.Forms;
using Utilities.System;

namespace Utilities.UI.Helper
{
    public static class PathDialog
    {
        public static string ChooseDir(string title, string path)
        {
            using (var openDialog = new FolderBrowserDialog())
            {
                openDialog.Description = title;
                openDialog.SelectedPath = DirUtils.GetDirectory(path);
                openDialog.ShowNewFolderButton = false;
                var result = openDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    path = openDialog.SelectedPath;
                }
            }
            return path;
        }

        public static string OpenFile(string title, string filter, string pattern, string path, int index = 1)
        {
            var firstFile = FileUtils.GetFirstFile(path, pattern);
            path = firstFile is null ? DirUtils.MyDocuments : firstFile;

            using (var openDialog = new OpenFileDialog())
            {
                openDialog.Title = title;
                openDialog.Filter = filter;
                openDialog.FilterIndex = index;
                openDialog.CheckFileExists = true;
                openDialog.Multiselect = false;
                openDialog.InitialDirectory = GetDirectoryName(path);
                var result = openDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    path = openDialog.FileName;
                }
            }
            return path;
        }

        public static string GetDirectoryName(string directory)
        {
            directory = DirUtils.GetDirectory(directory);
            return Path.GetDirectoryName(directory);
        }
    }
}
