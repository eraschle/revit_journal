using RevitJournal.Duplicate.Comparer;
using System.Windows.Media;

namespace RevitJournalUI.MetadataUI.Converter
{
    public static class ConverterColors
    {
        public static readonly SolidColorBrush TransparentBackground = new SolidColorBrush(Colors.Transparent);
        public static readonly SolidColorBrush DefaultBackground = new SolidColorBrush(Colors.WhiteSmoke);
        public static readonly SolidColorBrush ErrorBackground = new SolidColorBrush(Colors.Tomato);
        public static readonly SolidColorBrush ValidBackground = new SolidColorBrush(Colors.LawnGreen);
        public static readonly SolidColorBrush ValidDefaultComparerBackground = new SolidColorBrush(Colors.GreenYellow);
        public static readonly SolidColorBrush Magenta = new SolidColorBrush(Colors.Magenta);

        public static SolidColorBrush GetSourceColor<TModel>(TModel original, TModel source, IDuplicateComparer<TModel> comparer) where TModel : class
        {
            if (original is null || source is null || comparer is null) { return DefaultBackground; }
            if (comparer.UseComparer)
            {
                return DefaultBackground;
            }
            return GetModelColor(original, source, comparer);
        }

        public static SolidColorBrush GetModelColor<TModel>(TModel original, TModel source, ILevenstheinComparer<TModel> comparer) where TModel : class
        {
            if (original is null || source is null || comparer is null) { return DefaultBackground; }

            if (comparer.Equals(original, source))
            {
                return ValidBackground;
            }
            return ErrorBackground;
        }

        public static SolidColorBrush GetOriginalColor<TModel>(IDuplicateComparer<TModel> comparer) where TModel : class
        {
            if (comparer is null) { return TransparentBackground; }

            if (comparer.UseComparer)
            {
                return ValidDefaultComparerBackground;
            }
            return DefaultBackground;
        }
    }
}
