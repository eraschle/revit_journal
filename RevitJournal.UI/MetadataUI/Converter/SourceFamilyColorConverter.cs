using DataSource.Model.Family;
using RevitJournal.Duplicate.Comparer;
using System;
using System.Globalization;
using System.Linq;

namespace RevitJournalUI.MetadataUI.Converter
{
    public class SourceFamilyColorConverter : AColorConverter<Family>
    {
        private const string PrefixParameter = "Src";
        private const string ModelComparer = "Model";
        public SourceFamilyColorConverter() : base(new FamilyDuplicateComparer()) { }

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values is null || values.Any(val => val is null)) { return ConverterColors.TransparentBackground; }
            var parameterName = values[2].ToString().Replace(PrefixParameter, string.Empty);

            var comparer = Comparer.ByName(parameterName);
            var original = (values[0] as Family);
            var source = (values[1] as Family);
            if (parameterName.Equals(ModelComparer, StringComparison.CurrentCulture))
            {
                comparer = Comparer.ByName("Name");
            }
            return ConverterColors.GetSourceColor(original, source, comparer);
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
