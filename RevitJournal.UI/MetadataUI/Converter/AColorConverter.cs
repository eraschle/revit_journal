using RevitJournal.Duplicate.Comparer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace RevitJournalUI.MetadataUI.Converter
{
    public abstract class AColorConverter<TModel> : IMultiValueConverter
    {
        protected IModelDuplicateComparer<TModel> Comparer { get; private set; }

        protected AColorConverter(IModelDuplicateComparer<TModel> comparer)
        {
            Comparer = comparer;
        }

        public void SetComparers(ICollection<IDuplicateComparer<TModel>> comparers)
        {
            Comparer.PropertyComparers = comparers;
        }

        public abstract object Convert(object[] values, Type targetType, object parameter, CultureInfo culture);
        public abstract object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture);
    }
}
