using DataSource.Model.Family;
using RevitJournal.Duplicate.Comparer;
using System;
using System.Globalization;
using System.Linq;

namespace RevitJournalUI.MetadataUI.Converter
{
    public class SourceFamilyTypeParameterColorConverter : AColorConverter<Parameter>
    {
        public SourceFamilyTypeParameterColorConverter() : base(new ParameterDuplicateComparer()) { }

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values is null || values.Any(val => val is null)) { return ConverterColors.TransparentBackground; }

            var parameterName = values[3].ToString();
            var parameterNameValue = values[2].ToString();

            var comparer = Comparer.ByName(parameterName);
            var original = (values[0] as FamilyType).ByName(parameterNameValue);
            var source = (values[1] as FamilyType).ByName(parameterNameValue);

            return ConverterColors.GetSourceColor(original, source, comparer);
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
