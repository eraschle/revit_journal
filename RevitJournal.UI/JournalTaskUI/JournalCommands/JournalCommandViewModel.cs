using RevitJournal.Journal;
using RevitJournal.Journal.Command;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace RevitJournalUI.JournalTaskUI.JournalCommands
{
    public class JournalCommandViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public IJournalCommand Command { get; set; }

        public string CommandName { get { return Command.Name; } }

        private Visibility _ParameterVisibility = Visibility.Collapsed;
        public Visibility ParameterVisibility
        {
            get { return _ParameterVisibility; }
            set
            {
                if (_ParameterVisibility == value) { return; }

                _ParameterVisibility = value;
                OnPropertyChanged(nameof(ParameterVisibility));
            }
        }

        public ObservableCollection<ACmdParameterViewModel> Parameters { get; }
            = new ObservableCollection<ACmdParameterViewModel>();

        public virtual void UpdateJournalCommandParameters()
        {
            if (Command.HasParameters == false) { return; }

            var rootDirectory = string.Empty;
            if (TaskManager.HasRootDirectory(out var root))
            {
                rootDirectory = root.FullPath;
            }

            Parameters.Clear();
            CmdParameterInfoViewModel infoModel = null;
            foreach (var parameter in Command.Parameters)
            {
                ACmdParameterViewModel viewModel = null;
                switch (parameter.ParameterType)
                {
                    case JournalParameterType.Info:
                        infoModel = new CmdParameterInfoViewModel
                        {
                            CommandParameter = parameter
                        };
                        viewModel = infoModel;
                        break;
                    case JournalParameterType.String:
                        viewModel = new CmdParameterStringViewModel
                        {
                            CommandParameter = parameter
                        };
                        break;
                    case JournalParameterType.TextFile:
                        var txtTitle = "Select A Shared Parameter File";
                        var txtFileFilter = "Text Files (*.txt)|*.txt";
                        var txtSearch = "*.txt";
                        var txtOptions = SearchOption.AllDirectories;
                        var firstTextFile = Directory.GetFiles(rootDirectory, txtSearch, txtOptions).FirstOrDefault();
                        if (firstTextFile is null)
                        {
                            firstTextFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        }
                        viewModel = new CmdParameterFileViewModel(txtTitle, txtFileFilter, 1)
                        {
                            CommandParameter = parameter,
                            ParameterValue = firstTextFile
                        };
                        break;
                    case JournalParameterType.ImageFile:
                        var imageTitle = "Select A Shared Parameter File";
                        var imageFileFilter = "PNG (*.png)|*.png|JPG (*.jpg)|*.jpg|BMP (*.bmp)|*.bmp|All (*.*)|*.*";
                        var imageSearch = "*png";
                        var imageOptions = SearchOption.AllDirectories;
                        var firstImageFile = Directory.GetFiles(rootDirectory, imageSearch, imageOptions).FirstOrDefault();
                        if (firstImageFile is null)
                        {
                            firstImageFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        }
                        viewModel = new CmdParameterFileViewModel(imageTitle, imageFileFilter, 1)
                        {
                            CommandParameter = parameter,
                            ParameterValue = firstImageFile
                        };
                        break;
                    case JournalParameterType.Folder:
                        viewModel = new CmdParameterFolderViewModel
                        {
                            CommandParameter = parameter,
                            ParameterValue = rootDirectory
                        };
                        break;
                    case JournalParameterType.Boolean:
                        viewModel = new CmdParameterBooleanViewModel
                        {
                            CommandParameter = parameter
                        };
                        break;
                    case JournalParameterType.List:
                        viewModel = new CmdParameterListViewModel
                        {
                            CommandParameter = parameter
                        };
                        break;
                    case JournalParameterType.Select:
                        if (!(parameter is CommandParameterExternalSelect select)) { break; }
                        var selectViewModel = new CmdParameterSelectViewModel
                        {
                            CommandParameter = parameter,
                        };
                        selectViewModel.UpdateSelectableValues(select.SelectableValues);
                        viewModel = selectViewModel;
                        break;
                }
                if (viewModel is null) { continue; }

                Parameters.Add(viewModel);
            }
            if (infoModel != null)
            {
                foreach (var parameter in Parameters)
                {
                    if (parameter == infoModel) { continue; }

                    parameter.PropertyChanged += new PropertyChangedEventHandler(infoModel.OnOtherParameterChanged);
                }
            }
        }

        private bool _Checked = false;
        public bool Checked
        {
            get { return _Checked; }
            set
            {
                if (_Checked == value) { return; }

                _Checked = value;
                OnPropertyChanged(nameof(Checked));
                SetParameterVisibility();
            }
        }

        private bool _Enabled = true;
        public bool Enabled
        {
            get { return _Enabled; }
            set
            {
                if (_Enabled == value) { return; }

                _Enabled = value;
                OnPropertyChanged(nameof(Enabled));
            }
        }

        private void SetParameterVisibility()
        {
            ParameterVisibility = Command.HasParameters && Checked ? Visibility.Visible : Visibility.Collapsed;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
