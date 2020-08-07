using System;
using System.Globalization;

namespace DataSource.Helper
{
    public static class DateHelper
    {
        public static string AsString(DateTime date)
        {
            //var cultur = CultureInfo.GetCultureInfo("DE-ch");
            var cultur = CultureInfo.CurrentCulture;
            return date.ToString(cultur.DateTimeFormat);
        }

        public static DateTime AsDate(string datetime)
        {
            return DateTime.TryParse(datetime, out var date) ? date : DateTime.MinValue;
        }
    }
}
