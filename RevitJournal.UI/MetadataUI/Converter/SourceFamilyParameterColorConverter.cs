using DataSource.Model.Family;
using RevitJournal.Duplicate.Comparer;
using System;
using System.Globalization;
using System.Linq;

namespace RevitJournalUI.MetadataUI.Converter
{
    public class SourceFamilyParameterColorConverter : AColorConverter<Parameter>
    {
        public SourceFamilyParameterColorConverter() : base(new ParameterDuplicateComparer()) { }

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values is null || values.Any(val => val is null)) { return ConverterColors.TransparentBackground; }

            var parameterName = values[3].ToString();
            var parameterNameValue = values[2].ToString();

            var comparer = Comparer.ByName(parameterName);
            var originalParameter = (values[0] as Family).ByName(parameterNameValue);
            var sourceParameter = (values[1] as Family).ByName(parameterNameValue);

            return ConverterColors.GetSourceColor(originalParameter, sourceParameter, comparer);
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
