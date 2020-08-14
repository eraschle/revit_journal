using RevitAction.Action;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace RevitJournalUI.Tasks.Actions.Parameter
{
    public static class ParameterVmBuilder
    {
        public static IParameterViewModel Build(IActionParameter parameter)
        {
            if (parameter is null) { return null; }

            switch (parameter.Kind)
            {
                case ParameterKind.InfoDynamic:
                case ParameterKind.TextInfoValue:
                    return new InfoParameterViewModel(parameter);
                case ParameterKind.TextValue:
                    return new ParameterViewModel(parameter);
                case ParameterKind.Boolean:
                    return new BoolParameterViewModel(parameter);
                case ParameterKind.TextFile:
                    return BuildTextFile(parameter);
                case ParameterKind.ImageFile:
                    return BuildImageFile(parameter);
                case ParameterKind.SelectFolder:
                    return BuildSelectFolder(parameter);
                case ParameterKind.List:
                    //viewModel = new CmdParameterListViewModel
                    //{
                    //    Parameter = parameter
                    //};
                    //break;
                case ParameterKind.Selectable:
                    //if (!(parameter is CommandParameterExternalSelect select)) { break; }
                    //var selectViewModel = new CmdParameterSelectViewModel
                    //{
                    //    Parameter = parameter,
                    //};
                    //selectViewModel.UpdateSelectableValues(select.SelectableValues);
                    //viewModel = selectViewModel;
                    //break;
                case ParameterKind.Hidden:
                default:
                    return null;
            }
        }

        private static ParameterViewModel BuildTextFile(IActionParameter parameter)
        {
            return new FileParameterViewModel(parameter)
            {
                Title = "Select A Shared Parameter File",
                Filter = "Text Files (*.txt)|*.txt",
                Pattern = "*.txt"
            };
        }

        private static ParameterViewModel BuildImageFile(IActionParameter parameter)
        {
            return new FileParameterViewModel(parameter)
            {
                Title = "Select A Shared Parameter File",
                Filter = "PNG (*.png)|*.png|JPG (*.jpg)|*.jpg|BMP (*.bmp)|*.bmp|All (*.*)|*.*",
                Pattern = "*.png"
            };
        }

        private static ParameterViewModel BuildSelectFolder(IActionParameter parameter)
        {
            return new FolderParameterViewModel(parameter)
            {
                Title = "Select a Folder"
            };
        }

        public static void ConnectInfo(IEnumerable<IParameterViewModel> viewModels)
        {
            if (viewModels is null || HasInfoViewModel(viewModels, out var info) == false) { return; }

            foreach (var viewModel in viewModels)
            {
                if (viewModel == info) { continue; }

                viewModel.PropertyChanged += new PropertyChangedEventHandler(info.OnOtherChanged);
            }
        }


        public static bool HasInfoViewModel(IEnumerable<IParameterViewModel> viewModels, out IParameterViewModel model)
        {
            model = null;
            if (viewModels != null)
            {
                model = viewModels.FirstOrDefault(act => act.Kind == ParameterKind.InfoDynamic);
            }
            return model != null; ;
        }
    }
}
