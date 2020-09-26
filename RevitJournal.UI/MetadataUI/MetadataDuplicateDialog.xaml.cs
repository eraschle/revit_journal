using RevitJournalUI.Helper;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using RevitJournalUI.MetadataUI.Models;
using RevitJournalUI.MetadataUI.Converter;
using RevitJournal.Duplicate.Comparer;
using RevitJournal.Library;

namespace RevitJournalUI.MetadataUI
{
    /// <summary>
    /// Interaction logic for DublicatedManagerPageView.xaml
    /// </summary>
    public partial class MetadataDuplicateDialog : Window
    {

        private const string KeyOrigFamilyConverter = "BkgOriginalFamilyConverter";
        private const string KeyOrigFamilyParamConverter = "BkgOriginalFamilyParameterConverter";
        private const string KeyOrigFamilyTypeParamConverter = "BkgOriginalFamilyTypeParameterConverter";

        private const string KeySrcFamilyConverter = "BkgSourceFamilyConverter";
        private const string KeySrcFamilyParamConverter = "BkgSourceFamilyParameterConverter";
        private const string KeySrcFamilyTypParamConverter = "BkgSourceFamilyTypeParameterConverter";

        private MetadataDuplicateDialogModel ViewModel { get { return DataContext as MetadataDuplicateDialogModel; } }

        public MetadataDuplicateDialog(LibraryManager libraryManager)
        {
            InitializeComponent();
            ViewModel.SetRevitFiles(libraryManager);
        }

        private void LstRevitFiles_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.AddedItems.Count == 0) { return; }
            if (!(args.AddedItems[0] is OriginalFamilyViewModel model)) { return; }

            ViewModel.UpdateOriginal(model.Family);
            ListViewHelper.SizeColumns(lstOrigFamPar);

            ViewModel.Duplicated.Clear();
            foreach (var dublicate in model.DuplicateModels)
            {
                ViewModel.Duplicated.Add(dublicate);
            }
            if (ViewModel.Duplicated.Count > 0)
            {
                var dublicted = ViewModel.Duplicated[0];
                cbxDublicated.SelectedIndex = 0;
                ViewModel.UpdateSource(dublicted.Family);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CbxDublicated_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            ListViewHelper.SizeColumns(lstSrcFamPar);
            if (args.AddedItems.Count != 1) { return; }

            var selected = args.AddedItems[0];
            if (!(selected is DuplicateFamilyViewModel model)) { return; }

            ViewModel.UpdateSource(model.Family);
        }


        private void CbxSourceSelected_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListViewHelper.SizeColumns(lstSrcFamTypePar);
        }

        private void CbxOriginalSelected_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListViewHelper.SizeColumns(lstOrigFamTypePar);
        }

        private void Button_Use_Filter(object sender, RoutedEventArgs e)
        {
            SetConverter(KeyOrigFamilyConverter, ViewModel.FamilyInformationsComparers);
            SetConverter(KeyOrigFamilyParamConverter, ViewModel.FamilyParameterComparers);
            SetConverter(KeyOrigFamilyTypeParamConverter, ViewModel.FamilyTypeParametersComparers);
            SetConverter(KeySrcFamilyConverter, ViewModel.FamilyInformationsComparers);
            SetConverter(KeySrcFamilyParamConverter, ViewModel.FamilyParameterComparers);
            SetConverter(KeySrcFamilyTypParamConverter, ViewModel.FamilyTypeParametersComparers);
        }

        private void SetConverter<TModel>(string resourcesKey, ICollection<IDuplicateComparer<TModel>> comparers)
        {
            if (!(Resources[resourcesKey] is AColorConverter<TModel> converter)) { return; }

            converter.SetComparers(comparers);
        }
    }
}
